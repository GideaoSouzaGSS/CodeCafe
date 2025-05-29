using CodeCafe.ApiService.Features.Auth.Commands.Result;
using MediatR;

namespace  CodeCafe.ApiService.Features.Auth.Commands.TwoFactor;

public class HabilitarTwoFactorCommand : IRequest<CommandResult>
{
    public string Codigo { get; set; } // Código do app para verificação inicial
}