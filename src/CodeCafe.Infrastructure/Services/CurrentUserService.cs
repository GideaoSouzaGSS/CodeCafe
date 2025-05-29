using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using CodeCafe.Application.Interfaces.Services;
using CodeCafe.Data;

namespace CodeCafe.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly AppDbContext _context;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor, AppDbContext context)
    {
        _httpContextAccessor = httpContextAccessor;
        _context = context;
    }

    public Guid UserId
    {
        get
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(userIdClaim, out var usuarioid))
            {
                return usuarioid;             
            }
            return Guid.Empty; // Retorna Guid.Empty se o ID do usuário não for encontrado ou inválido
        }
    }

   public Guid ProfileId
    {
        get
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(userIdClaim, out var userId))
            {
                var user = _context.UserProfiles.FirstOrDefault(p => p.UsuarioId == userId); 
                if (user != null)
                {
                    return user.Id; // Retorna o ID do perfil associado ao usuário
                }
            }
            return Guid.Empty; 
        }
    }
    public string Username => 
        _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;

    public string Email => 
        _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;

    public bool IsAuthenticated => 
        _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    public IEnumerable<string> Roles => 
        _httpContextAccessor.HttpContext?.User?.FindAll(ClaimTypes.Role).Select(c => c.Value) ?? Enumerable.Empty<string>();

}