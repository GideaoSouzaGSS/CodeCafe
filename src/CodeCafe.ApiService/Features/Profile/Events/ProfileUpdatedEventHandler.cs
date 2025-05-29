using MediatR;
using CodeCafe.Application.Interfaces.Services;
using CodeCafe.Data;
using CodeCafe.Data.Store;

namespace CodeCafe.ApiService.Features.Profile.Events;

public class ProfileUpdatedEventHandler : INotificationHandler<ProfileUpdatedEvent>
{
    private readonly AppDbContext _appDbContext;
    private readonly IEventStore _eventStoreDbContext;
    private readonly ICurrentUserService _currentUserService;

    public ProfileUpdatedEventHandler(
        AppDbContext appDbContext,
        IEventStore eventStoreDbContext,
        ICurrentUserService currentUserService)
    {
        _appDbContext = appDbContext;
        _eventStoreDbContext = eventStoreDbContext;
        _currentUserService = currentUserService;
    }

    public async Task Handle(ProfileUpdatedEvent @event, CancellationToken cancellationToken)
    {
        await _eventStoreDbContext.SaveEventAsync(@event);
    }
}