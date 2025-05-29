using CodeCafe.Domain.Entities;

namespace CodeCafe.Domain.Interfaces;

public interface IProfileRepository
{
    // Task<List<UserProfile>> GetAllProfilesAsync(Guid currentUserId, CancellationToken ct);
    Task<Guid> CreateProfileForUserAsync(Guid userId, CancellationToken ct);
    Task<UserProfile?> GetUserProfileAsync(Guid userId, CancellationToken ct);
    Task UpdateAsync(UserProfile profile, CancellationToken ct);

}