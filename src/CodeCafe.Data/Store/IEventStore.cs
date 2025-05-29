namespace CodeCafe.Data.Store;

public interface IEventStore
{
    Task SaveEventAsync(object @event);
    Task<List<object>> GetEventsAsync(Guid aggregateId); // UsuarioId é o AggregateId
    Task<List<EventData>> GetAllEventsAsync();
}