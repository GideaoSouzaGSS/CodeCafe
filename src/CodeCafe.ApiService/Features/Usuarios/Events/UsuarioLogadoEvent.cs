using MediatR;

namespace CodeCafe.ApiService.Features.Usuarios.Models;
public record UsuarioLogadoEvent(
    Guid UsuarioId,
    DateTime DataLogin
): INotification;