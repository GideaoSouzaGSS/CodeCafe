using CodeCafe.Domain.Entities;

namespace CodeCafe.ApiService.Features.Auth.Services;

public interface IJwtTokenService
{
    /// <summary>
    /// Gera um token JWT para o usuário especificado
    /// </summary>
    /// <param name="usuario">O usuário para o qual gerar o token</param>
    /// <returns>String contendo o token JWT</returns>
    string GenerateToken(Usuario usuario);
}