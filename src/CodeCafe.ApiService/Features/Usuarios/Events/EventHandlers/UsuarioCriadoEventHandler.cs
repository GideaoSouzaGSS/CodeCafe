using MediatR;
using CodeCafe.ApiService.Features.Usuarios.Events;
using CodeCafe.Data.Store;

namespace CodeCafe.ApiService.Features.Usuarios.EventHandlers;

public class UsuarioCriadoEventHandler : INotificationHandler<UsuarioCriadoEvent>
{
    private readonly IEventStore _eventStore;

    public UsuarioCriadoEventHandler(IEventStore eventStore)
    {
        _eventStore = eventStore;
    }

    public async Task Handle(UsuarioCriadoEvent @event, CancellationToken cancellationToken)
    {
        await _eventStore.SaveEventAsync(@event);
    }
}