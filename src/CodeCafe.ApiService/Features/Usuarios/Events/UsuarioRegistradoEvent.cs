using MediatR;

namespace CodeCafe.ApiService.Features.Usuarios.Models;

public record UsuarioRegistradoEvent(
    Guid UsuarioId,
    string Nome,
    string Email,
    DateTime DataNascimento,
    DateTime DataRegistro
): INotification;