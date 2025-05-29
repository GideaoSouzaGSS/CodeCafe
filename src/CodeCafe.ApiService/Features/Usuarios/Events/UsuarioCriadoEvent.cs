using MediatR;

namespace CodeCafe.ApiService.Features.Usuarios.Events;

public record UsuarioCriadoEvent(
    int UsuarioId,
    string Nome
) : INotification;