namespace CodeCafe.Application.Interfaces.Services;

public interface IEmailService
{
    Task EnviarEmailConfirmacaoAsync(string destinatario, string nome, string codigoConfirmacao);
}