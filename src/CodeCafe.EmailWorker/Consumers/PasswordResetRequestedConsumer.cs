// CodeCafe.EmailWorker/Consumers/PasswordResetRequestedConsumer.cs
using CodeCafe.Application.Interfaces.Services;
using CodeCafe.Messaging.Messages;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace CodeCafe.EmailWorker.Consumers;

public class PasswordResetRequestedConsumer : IConsumer<PasswordResetRequestedEvent>
{
    private readonly IEmailService _emailService;
    private readonly ILogger<PasswordResetRequestedConsumer> _logger;

    public PasswordResetRequestedConsumer(
        IEmailService emailService,
        ILogger<PasswordResetRequestedConsumer> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<PasswordResetRequestedEvent> context)
    {
        var message = context.Message;
        
        try
        {
            _logger.LogInformation("Sending password reset email to {Email}", message.Email);
            
            // Extrair o nome do usuário do email como fallback
            string userName = message.Email.Split('@')[0];
            
            // Usar o método existente do EmailService
            await _emailService.EnviarEmailConfirmacaoAsync(
                destinatario: message.Email,
                nome: userName,
                codigoConfirmacao: message.ResetToken
            );
            
            _logger.LogInformation("Password reset email sent successfully to {Email}", message.Email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending password reset email to {Email}", message.Email);
            throw;
        }
    }
}