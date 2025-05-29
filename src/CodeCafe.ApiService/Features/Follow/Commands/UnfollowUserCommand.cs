using MediatR;

namespace CodeCafe.ApiService.Features.Follow.Commands;

public record UnfollowUserCommand(Guid FollowerId, Guid UserToUnfollowId) : IRequest;