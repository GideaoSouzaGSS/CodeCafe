using MediatR;
using CodeCafe.ApiService.Features.Usuarios.Models;
using CodeCafe.Data.Store;

namespace CodeCafe.ApiService.Features.Auth.Events;

public class UsuarioLogadoEventHandler : INotificationHandler<UsuarioLogadoEvent>
{
    private readonly IEventStore _eventStore;

    public UsuarioLogadoEventHandler(IEventStore eventStore)
    {
        _eventStore = eventStore;
    }

    public async Task Handle(UsuarioLogadoEvent @event, CancellationToken cancellationToken)
    {
        await _eventStore.SaveEventAsync(@event);
    }
}