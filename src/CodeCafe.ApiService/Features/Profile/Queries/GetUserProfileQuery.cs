using MediatR;

namespace CodeCafe.ApiService.Features.Profile.Queries;

public record GetUserProfileQuery(Guid UserId) : IRequest<UserProfileDto>;