using MediatR;

namespace CodeCafe.ApiService.Features.Follow.Commands;
public record FollowUserCommand(Guid FollowerId, Guid UserToFollowId) : IRequest;