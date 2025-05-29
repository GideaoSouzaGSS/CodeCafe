using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using CodeCafe.Application.Interfaces.Services;
using System.IO;

namespace CodeCafe.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly string _smtpServer;
    private readonly int _smtpPort;
    private readonly string _smtpUsername;
    private readonly string _smtpPassword;
    private readonly string _fromEmail;
    private readonly string _fromName;
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
        _smtpServer = configuration["Email:SmtpServer"];
        _smtpPort = int.Parse(configuration["Email:SmtpPort"]);
        _smtpUsername = configuration["Email:Username"];
        _smtpPassword = configuration["Email:Password"];
        _fromEmail = configuration["Email:FromAddress"];
        _fromName = configuration["Email:FromName"];
    }

    public async Task EnviarEmailConfirmacaoAsync(string destinatario, string nome, string codigoConfirmacao)
    {
        if (string.IsNullOrEmpty(destinatario) || string.IsNullOrEmpty(nome) || string.IsNullOrEmpty(codigoConfirmacao))
        {
            throw new ArgumentException("Destinatário, nome e código de confirmação não podem ser nulos ou vazios.");
        }
        
        // Obter o template HTML do arquivo (pode ser otimizado para ler apenas uma vez na inicialização)
        string baseUrl = _configuration["Email:BaseUrl"] ?? "http://localhost:4200";
        string templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", "email-confirmation-template.html");
        string htmlTemplate;
        
        try 
        {
            htmlTemplate = File.ReadAllText(templatePath);
        }
        catch (Exception ex)
        {
            // Se não conseguir ler o arquivo, use um template simples
            htmlTemplate = GetFallbackEmailTemplate();
            // Log do erro
            Console.WriteLine($"Erro ao ler template de email: {ex.Message}");
        }
        
        // Substituir placeholders do template
        string urlConfirmacao = $"{baseUrl}/confirmar-email?codigo={codigoConfirmacao}&email={Uri.EscapeDataString(destinatario)}";
        
        string emailBody = htmlTemplate
            .Replace("{{user_name}}", nome)
            .Replace("{{company_name}}", "CodeCafe")
            .Replace("{{confirmation_link}}", urlConfirmacao)
            .Replace("9876b0926c2d4883acda76ad5bead3dd", codigoConfirmacao);
        
        var assunto = "Confirme seu email no CodeCafe";
        
        await EnviarEmailAsync(destinatario, assunto, emailBody);
    }

    private async Task EnviarEmailAsync(string destinatario, string assunto, string corpo)
    {
        using var message = new MailMessage
        {
            From = new MailAddress(_fromEmail, _fromName),
            Subject = assunto,
            Body = corpo,
            IsBodyHtml = true
        };
        
        message.To.Add(new MailAddress(destinatario));
        
        using var client = new SmtpClient(_smtpServer, _smtpPort)
        {
            Credentials = new NetworkCredential(_smtpUsername, _smtpPassword),
            EnableSsl = false
        };
        
        await client.SendMailAsync(message);
    }

    // Método para fornecer um template de fallback caso o arquivo não exista
    private string GetFallbackEmailTemplate()
    {
        return @"
        <!DOCTYPE html>
        <html>
        <body style='font-family: Arial, sans-serif; margin: 0; padding: 20px;'>
            <div style='max-width: 600px; margin: 0 auto; background-color: #f7f7f7; padding: 20px; border-radius: 5px;'>
                <h2>Olá {{user_name}},</h2>
                <p>Obrigado por se cadastrar no CodeCafe. Para confirmar seu email, clique no link abaixo:</p>
                <p><a href='{{confirmation_link}}' style='background-color: #4CAF50; color: white; padding: 10px 15px; text-decoration: none; border-radius: 4px;'>Confirmar meu email</a></p>
                <p>Ou use o código: <strong>{{confirmation_code}}</strong></p>
                <p>Se você não se cadastrou no CodeCafe, ignore este email.</p>
                <p>Atenciosamente,<br>Equipe CodeCafe</p>
            </div>
        </body>
        </html>"
        .Replace("{{confirmation_code}}", "9876b0926c2d4883acda76ad5bead3dd");
    }
}