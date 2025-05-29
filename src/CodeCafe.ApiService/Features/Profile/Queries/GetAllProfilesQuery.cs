using MediatR;

namespace CodeCafe.ApiService.Features.Profile.Queries;

public record GetAllProfilesQuery : IRequest<List<ProfileDto>>;

public record ProfileDto(
    Guid Id,
    string Username,
    string DisplayName,
    string Bio,
    string? PhotoUrl,
    string? CoverPhotoUrl,
    int FollowersCount,
    int FollowingCount,
    bool IsFollowing);