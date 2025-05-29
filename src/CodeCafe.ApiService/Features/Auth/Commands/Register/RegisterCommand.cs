using MediatR;

namespace CodeCafe.ApiService.Features.Auth.Commands.Login;
public record RegisterCommand(
    string Nome,
    string Email,
    string Senha,
    DateOnly DataNascimento
) : IRequest<Guid>; 