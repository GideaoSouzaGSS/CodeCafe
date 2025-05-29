using MediatR;
using CodeCafe.Application.Interfaces.Services;
using CodeCafe.Data;
using CodeCafe.Data.Store;

namespace CodeCafe.ApiService.Features.Follow.Events;
public class FollowUserCommandHandler : INotificationHandler<UserFollowedEvent>
{
    private readonly AppDbContext _appDbContext;
    private readonly IEventStore _eventStoreDbContext;
    private readonly ICurrentUserService _currentUserService;

    public FollowUserCommandHandler(
        AppDbContext appDbContext,
        IEventStore eventStoreDbContext,
        ICurrentUserService currentUserService)
    {
        _appDbContext = appDbContext;
        _eventStoreDbContext = eventStoreDbContext; 
        _currentUserService = currentUserService;
    }

    public async Task Handle(UserFollowedEvent @command, CancellationToken cancellationToken)
    {
        
        await _eventStoreDbContext.SaveEventAsync(@command);

        // 8. Publica o evento no barramento de mensagens
        // await _messageBus.PublishAsync(@event);
    }
}