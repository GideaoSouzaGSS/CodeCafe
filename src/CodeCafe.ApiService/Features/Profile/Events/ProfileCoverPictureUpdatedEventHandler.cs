using MediatR;
using CodeCafe.Data.Store;

namespace CodeCafe.ApiService.Features.Profile.Events;

public class ProfileCoverPictureUpdatedEventHandler : INotificationHandler<ProfileCoverPictureUpdatedEvent>
{
    private readonly IEventStore _eventStore;

    public ProfileCoverPictureUpdatedEventHandler(IEventStore eventStore)
    {
        _eventStore = eventStore;
    }

    public async Task Handle(ProfileCoverPictureUpdatedEvent notification, CancellationToken cancellationToken)
    {
        await _eventStore.SaveEventAsync(notification);
    }
}