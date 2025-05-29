using MediatR;
using System;

namespace CodeCafe.ApiService.Features.Profile.Commands;

public record UpdateProfileCommand(
    Guid UserId,
    string Username,       // Nome de usuário (pode ser alterado)
    string? DisplayName,
    string? Bio,
    bool AcceptFollow,
    bool AcceptDirectMessage
) : IRequest<bool>; // Retorna true se a atualização for bem-sucedida, false caso contrário.
                    // Alternativamente, poderia ser IRequest<Unit> se nenhum resultado específico for necessário.
