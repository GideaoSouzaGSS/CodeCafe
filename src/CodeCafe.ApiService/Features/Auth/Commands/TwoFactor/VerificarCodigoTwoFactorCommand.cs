using CodeCafe.ApiService.Features.Auth.Commands.Result;
using MediatR;

namespace CodeCafe.ApiService.Features.Auth.Commands.TwoFactor;

/// <summary>
/// Comando para verificar um código de autenticação de dois fatores
/// </summary>
public class VerificarCodigoTwoFactorCommand : IRequest<CommandResult>
{
    /// <summary>
    /// Email do usuário
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// Código de verificação (do app ou token de recuperação)
    /// </summary>
    public string Codigo { get; set; }
}