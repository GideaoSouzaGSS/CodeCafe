public abstract record MessageEvent(Guid EventId, DateTime Timestamp);

public record MessageSentEvent(
    Guid MessageId,
    Guid UsuarioId,
    Guid RecipientId,
    string Content) : MessageEvent(Guid.NewGuid(), DateTime.Now);
