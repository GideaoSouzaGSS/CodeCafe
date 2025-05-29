// CodeCafe.EmailWorker/Consumers/UserRegisteredConsumer.cs
using CodeCafe.Application.Interfaces.Services;
using CodeCafe.Messaging.Messages;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace CodeCafe.EmailWorker.Consumers;

public class ResenEmailConfirmationConsumer : IConsumer<ResendEmailConfirmationEvent>
{
    private readonly IEmailService _emailService;
    private readonly ILogger<UserRegisteredConsumer> _logger;

    public ResenEmailConfirmationConsumer(
        IEmailService emailService,
        ILogger<UserRegisteredConsumer> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ResendEmailConfirmationEvent> context)
    {
        var message = context.Message;
        
        try
        {
            _logger.LogInformation("Sending email confirmation to {Email}", message.Email);
            
            // Obter a URL base da configuração ou usar um valor padrão
            string confirmationLink = $"{message.ConfirmationToken}";
            
            // Usar o método existente do EmailService
            await _emailService.EnviarEmailConfirmacaoAsync(
                destinatario: message.Email,
                nome: message.Username,
                codigoConfirmacao: message.ConfirmationToken
            );
            
            _logger.LogInformation("Email confirmation sent successfully to {Email}", message.Email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending confirmation email to {Email}", message.Email);
            // Se falhar, a mensagem voltará para a fila (retry policy)
            throw;
        }
    }
}