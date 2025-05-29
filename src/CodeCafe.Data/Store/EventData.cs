namespace CodeCafe.Data.Store;

public class EventData
{
    public Guid Id { get; set; }
    public Guid AggregateId { get; set; } // UsuarioId
    public string EventType { get; set; }
    public string Payload { get; set; }
    public DateTime Timestamp { get; set; }
}