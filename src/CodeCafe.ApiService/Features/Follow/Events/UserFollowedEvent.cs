

using MediatR;

namespace CodeCafe.ApiService.Features.Follow.Events;

public record UserFollowedEvent(
    Guid EventId,
    Guid FollowerId,
    Guid FollowedId,
    DateTime OccurredAt) : INotification;