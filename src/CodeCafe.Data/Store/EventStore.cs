using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace CodeCafe.Data.Store;
public class EventStoreRepository : IEventStore 
{ 
    private readonly EventStoreDbContext _context;

    public EventStoreRepository(EventStoreDbContext context)
    {
        _context = context;
    }

    public async Task SaveEventAsync(object @event)
    {
        try
        {

            var eventData = new CodeCafe.Data.Store.EventData
            {
                AggregateId = ((dynamic)@event).UsuarioId, // Assume que o evento tem UsuarioId
                EventType = @event.GetType().Name,
                Payload = JsonSerializer.Serialize(@event),
                Timestamp = DateTime.UtcNow
            };
            _context.Events.Add(eventData);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // Log the exception or handle it as needed

            var eventData = new CodeCafe.Data.Store.EventData
            {
                AggregateId = Guid.NewGuid(), // Assume que o evento tem UsuarioId
                EventType = @event.GetType().Name,
                Payload = JsonSerializer.Serialize(@event),
                Timestamp = DateTime.UtcNow
            };
            _context.Events.Add(eventData);
            await _context.SaveChangesAsync();
        }


    }

    public async Task<List<object>> GetEventsAsync(Guid aggregateId)
    {
        var events = await _context.Events
            .Where(e => e.AggregateId == aggregateId)
            .OrderBy(e => e.Timestamp)
            .ToListAsync();

        return events.Select(e => JsonSerializer.Deserialize<object>(e.Payload)).ToList();
    }

    public async Task<List<EventData>> GetAllEventsAsync()
    {
        var events = await _context.Events
            .OrderByDescending(e => e.Timestamp)
            .ToListAsync();

        return events;
    }
}