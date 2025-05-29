using Microsoft.EntityFrameworkCore;
using CodeCafe.Domain.Entities;
using CodeCafe.Domain.Interfaces;

namespace CodeCafe.Data.Repositories;

public class ProfileRepository : IProfileRepository 
{
    private readonly AppDbContext _context;

    public ProfileRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<UserProfile?> GetUserProfileAsync(Guid currentUserId, CancellationToken ct)
    {
        var user = await _context.UserProfiles
            .Include(p => p.Followers)
            .Include(p => p.Followings)
            .FirstOrDefaultAsync(c => c.Id == currentUserId, ct);

        if (user == null)
        {
            throw new Exception("User profile not found");
        }
        return user;
    }
    public async Task<Guid> CreateProfileForUserAsync(Guid userId, CancellationToken ct)
    {
        // Cria o perfil usando o método Create da entidade
        var newProfile = UserProfile.Create(
            usuarioId: userId,
            username: string.Empty, // Gera um nome de usuário padrão
            displayName: string.Empty,
            bio: string.Empty,
            acceptFollow: true, // Aceita seguidores por padrão
            acceptDirectMessage: true // Aceita mensagens diretas por padrão
        );

        // Adiciona o perfil ao contexto do EF Core
        await _context.UserProfiles.AddAsync(newProfile, ct);
        await _context.SaveChangesAsync(ct);

        // Retorna o ID do novo perfil
        return newProfile.Id;
    }

    public async Task UpdateAsync(UserProfile profile, CancellationToken ct) 
    {
        // Certifique-se de que a entidade está sendo rastreada pelo contexto
        var existingProfile = await _context.UserProfiles
            .FirstOrDefaultAsync(p => p.Id == profile.Id, ct);

        if (existingProfile == null)
            throw new Exception("Profile not found.");

        // Atualiza os campos da entidade existente
        existingProfile.UpdateProfile(
            profile.Username,
            profile.DisplayName,
            profile.Bio,
            profile.AccepptFollow,
            profile.AcceptDirectMessage
        );

        // Salva as alterações no banco de dados
        await _context.SaveChangesAsync(ct);
    }
}