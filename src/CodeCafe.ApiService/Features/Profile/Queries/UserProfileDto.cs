namespace CodeCafe.ApiService.Features.Profile.Queries;

public record UserProfileDto(
    Guid Id,
    string Username,
    string DisplayName,
    string Bio,
    string PhotoUrl, 
    string CoverPhotoUrl,
    int FollowingCount,
    int FollowersCount,
    bool IsFollowing);