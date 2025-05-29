using MediatR;
using Microsoft.EntityFrameworkCore;
using CodeCafe.Application.Interfaces.Services;
using CodeCafe.Data;
using CodeCafe.Domain.Interfaces;

namespace CodeCafe.ApiService.Features.Profile.Queries;

public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, UserProfileDto>
{
    private readonly IProfileRepository _profileRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetUserProfileQueryHandler(ICurrentUserService currentUserService, IProfileRepository profileRepository)
    {
        _currentUserService = currentUserService;
        _profileRepository = profileRepository;
    }

    public async Task<UserProfileDto> Handle(GetUserProfileQuery query, CancellationToken ct)
    {
        var currentUserId = _currentUserService.ProfileId;

        // Verificar se o usuário atual não tem um perfil
        if (currentUserId == Guid.Empty)
        {
            // Criar um novo perfil para o usuário
            currentUserId = await _profileRepository.CreateProfileForUserAsync(_currentUserService.UserId, ct);
        }

        var profile = await _profileRepository.GetUserProfileAsync(currentUserId, ct);

        if (profile == null)
            throw new Exception("User profile not found");

        // Map UserProfile to UserProfileDto
        var profileDto = new UserProfileDto(
            profile.Id,
            profile.Username,
            profile.DisplayName,
            profile.Bio,
            profile.PhotoUrl ?? string.Empty,
            profile.CoverPhotoUrl ?? string.Empty,
            profile.Followings.Count,
            profile.Followers.Count,
            profile.Followers.Any(f => f.FollowerId == _currentUserService.UserId)
        );

        return profileDto;
    }
}