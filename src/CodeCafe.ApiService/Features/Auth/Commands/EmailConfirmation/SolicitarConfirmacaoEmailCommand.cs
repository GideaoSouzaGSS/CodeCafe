using CodeCafe.ApiService.Features.Auth.Commands.Result;
using MediatR;

namespace CodeCafe.ApiService.Features.Auth.Commands.EmailConfirmation;
public class SolicitarConfirmacaoEmailCommand : IRequest<CommandResult>
{
    public string Email { get; set; }
}