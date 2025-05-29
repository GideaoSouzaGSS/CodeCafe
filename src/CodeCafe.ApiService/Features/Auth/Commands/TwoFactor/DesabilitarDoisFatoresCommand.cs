using CodeCafe.ApiService.Features.Auth.Commands.Result;
using MediatR;

namespace CodeCafe.ApiService.Features.Auth.Commands.TwoFactor;

public class DesabilitarTwoFactorCommand : IRequest<CommandResult>
{
    public string Senha { get; set; } // Senha para verificar identidade antes de desabilitar
}