using MediatR;
using CodeCafe.ApiService.Features.Albums.Events;
using CodeCafe.Data.Store;

namespace CodeCafe.ApiService.Features.Profile.Events;

public class AlbumCreatedEventHandler : INotificationHandler<AlbumCreatedEvent>
{
    private readonly IEventStore _eventStoreDbContext;
    public AlbumCreatedEventHandler(
        IEventStore eventStoreDbContext)
    {
        _eventStoreDbContext = eventStoreDbContext;
    }

    public async Task Handle(AlbumCreatedEvent @event, CancellationToken cancellationToken)
    {
        await _eventStoreDbContext.SaveEventAsync(@event);
    }
}