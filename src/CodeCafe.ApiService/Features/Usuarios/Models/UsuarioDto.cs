namespace CodeCafe.ApiService.Features.Usuarios.Models;

public record UsuarioDto(
    Guid UsuarioId,
    string Nome,
    string Email,
    DateOnly DataNascimento
);