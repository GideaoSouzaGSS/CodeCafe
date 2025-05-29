
using CodeCafe.ApiService.Features.Auth.Commands.Result;
using MediatR;

namespace CodeCafe.ApiService.Features.Auth.Commands.EmailConfirmation;
public class ConfirmarEmailCommand : IRequest<CommandResult>
{
    public string Email { get; set; }
    public string Codigo { get; set; }
}
