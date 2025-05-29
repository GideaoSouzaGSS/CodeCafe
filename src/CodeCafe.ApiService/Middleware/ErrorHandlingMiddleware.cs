using System.Security;
using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace CodeCafe.ApiService.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;
    private readonly IWebHostEnvironment _env;

    public ErrorHandlingMiddleware(
        RequestDelegate next, 
        ILogger<ErrorHandlingMiddleware> logger,
        IWebHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Erro de validação: {RequestPath}", context.Request.Path);
            
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/problem+json";
            
            var errorResponse = new
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Title = "Erro de validação",
                Status = StatusCodes.Status400BadRequest,
                TraceId = context.TraceIdentifier,
                Timestamp = DateTime.UtcNow,
                Errors = ex.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => string.IsNullOrEmpty(g.Key) ? "_" : g.Key,
                        g => g.Select(e => new
                        {
                            Message = e.ErrorMessage,
                            ErrorCode = e.ErrorCode,
                            AttemptedValue = e.AttemptedValue
                        }).ToArray()
                    )
            };
            
            await context.Response.WriteAsJsonAsync(errorResponse);
        }
        catch (SecurityException ex)
        {
            _logger.LogWarning(ex, "Erro de segurança: {RequestPath} - {Message}", 
                context.Request.Path, ex.Message);
            
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/problem+json";
            
            var problemDetails = new ProblemDetails
            {
                Title = "Falha na autenticação",
                Detail = ex.Message,
                Status = StatusCodes.Status401Unauthorized,
                Type = "https://tools.ietf.org/html/rfc7235#section-3.1",
                Instance = context.Request.Path
            };
            
            // Adicionar propriedades extras ao problema
            problemDetails.Extensions["traceId"] = context.TraceIdentifier;
            problemDetails.Extensions["timestamp"] = DateTime.UtcNow;
            
            if (_env.IsDevelopment())
            {
                problemDetails.Extensions["stackTrace"] = ex.StackTrace;
                problemDetails.Extensions["source"] = ex.Source;
            }
            
            await context.Response.WriteAsJsonAsync(problemDetails);
        }
        catch (RedisConnectionException redisEx)
        {
            _logger.LogError(redisEx, "Erro de conexão com Redis: {Message}", redisEx.Message);
            
            context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            context.Response.ContentType = "application/problem+json";
            
            var problemDetails = new ProblemDetails
            {
                Title = "Serviço temporariamente indisponível",
                Detail = _env.IsDevelopment() 
                    ? $"Erro de conexão com Redis: {redisEx.Message}"
                    : "Não foi possível conectar ao serviço de cache",
                Status = StatusCodes.Status503ServiceUnavailable,
                Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.4",
                Instance = context.Request.Path
            };
            
            problemDetails.Extensions["traceId"] = context.TraceIdentifier;
            problemDetails.Extensions["timestamp"] = DateTime.UtcNow;
            problemDetails.Extensions["retryAfter"] = 30; // segundos
            
            if (_env.IsDevelopment())
            {
                problemDetails.Extensions["stackTrace"] = redisEx.StackTrace;
                problemDetails.Extensions["innerException"] = redisEx.InnerException?.Message;
            }
            
            await context.Response.WriteAsJsonAsync(problemDetails);
        }
        catch (Exception ex)
        {
            // Analisa exceções em profundidade para detectar problemas Redis
            var redisIssue = ContainsRedisConnectionIssue(ex);
            if (redisIssue.found)
            {
                _logger.LogError(ex, "Erro de conexão Redis (aninhado): {Message}", redisIssue.message);
                
                context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                context.Response.ContentType = "application/problem+json";
                
                var problemDetails = new ProblemDetails
                {
                    Title = "Serviço temporariamente indisponível",
                    Detail = _env.IsDevelopment()
                        ? $"Erro de conexão com o cache: {redisIssue.message}"
                        : "O sistema está temporariamente indisponível",
                    Status = StatusCodes.Status503ServiceUnavailable,
                    Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.4",
                    Instance = context.Request.Path
                };
                
                problemDetails.Extensions["traceId"] = context.TraceIdentifier;
                problemDetails.Extensions["timestamp"] = DateTime.UtcNow;
                problemDetails.Extensions["retryAfter"] = 30; // segundos
                
                await context.Response.WriteAsJsonAsync(problemDetails);
                return;
            }
            
            _logger.LogError(ex, "Erro não tratado: {RequestPath} - {Message}", 
                context.Request.Path, ex.Message);
            
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/problem+json";
            
            var internalErrorDetails = new ProblemDetails
            {
                Title = "Erro interno do servidor",
                Detail = _env.IsDevelopment() ? ex.Message : "Ocorreu um erro interno ao processar sua solicitação",
                Status = StatusCodes.Status500InternalServerError,
                Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1",
                Instance = context.Request.Path
            };
            
            internalErrorDetails.Extensions["traceId"] = context.TraceIdentifier;
            internalErrorDetails.Extensions["timestamp"] = DateTime.UtcNow;
            
            if (_env.IsDevelopment())
            {
                internalErrorDetails.Extensions["exception"] = new {
                    Message = ex.Message,
                    Type = ex.GetType().FullName,
                    StackTrace = ex.StackTrace,
                    Source = ex.Source,
                    InnerException = ex.InnerException != null ? new {
                        Message = ex.InnerException.Message,
                        Type = ex.InnerException.GetType().FullName,
                    } : null
                };
            }
            
            await context.Response.WriteAsJsonAsync(internalErrorDetails);
        }
    }
    
    private (bool found, string message) ContainsRedisConnectionIssue(Exception ex)
    {
        var currentEx = ex;
        while (currentEx != null)
        {
            if (currentEx is RedisConnectionException)
            {
                return (true, currentEx.Message);
            }
            
            if (currentEx.Message != null && (
                currentEx.Message.Contains("Redis", StringComparison.OrdinalIgnoreCase) ||
                currentEx.Message.Contains("secure connections", StringComparison.OrdinalIgnoreCase) ||
                currentEx.Message.Contains("StackExchange.Redis", StringComparison.OrdinalIgnoreCase) ||
                currentEx.Message.Contains("connection", StringComparison.OrdinalIgnoreCase)))
            {
                return (true, currentEx.Message);
            }
            
            currentEx = currentEx.InnerException;
        }
        
        return (false, null);
    }
}