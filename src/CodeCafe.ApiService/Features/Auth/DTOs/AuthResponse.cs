using CodeCafe.ApiService.Features.Usuarios.Models;

namespace CodeCafe.ApiService.Features.Auth.DTOs;

public class AuthResponse
{
    public string Token { get; set; }
    public UsuarioDto Usuario { get; set; }
    
    // Novas propriedades para 2FA
    public bool RequiresTwoFactor { get; set; }
    public string Email { get; set; }
    
    public AuthResponse() { }
    
    public AuthResponse(string token, UsuarioDto usuario)
    {
        Token = token;
        Usuario = usuario;
        RequiresTwoFactor = false;
    }
}