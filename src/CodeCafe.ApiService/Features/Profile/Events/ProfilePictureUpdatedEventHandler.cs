using MediatR;
using CodeCafe.Data;
using CodeCafe.Data.Store;

namespace CodeCafe.ApiService.Features.Profile.Events;

public class ProfilePictureUpdatedEventHandler : INotificationHandler<ProfilePictureUpdatedEvent>
{
    private readonly IEventStore _eventStore;

    public ProfilePictureUpdatedEventHandler(IEventStore eventStore)
    {
        _eventStore = eventStore;
    }

    public async Task Handle(ProfilePictureUpdatedEvent notification, CancellationToken cancellationToken)
    {
        await _eventStore.SaveEventAsync(notification);
    }
}