using CodeCafe.Domain.Events;

namespace  CodeCafe.Domain.Entities;

public class Message
{
    public Guid Id { get; private set; }
    public Guid ConversationId { get; private set; }
    public Conversation Conversation { get; private set; } = null!;
    public Guid SenderId { get; private set; }
    public Guid RecipientId { get; private set; }
    public string Content { get; private set; } = string.Empty;
    public DateTime SentAt { get; private set; }
    public bool IsRead { get; private set; }
    public DateTime? ReadAt { get; private set; }

    // Construtor privado para EF Core
    private Message() { }

    // Factory method para criação a partir de um evento
    public static Message CreateFromEvent(MessageSentEvent @event, Conversation conversation)
    {
        return new Message
        {
            Id = @event.MessageId, 
            Conversation = conversation,
            SenderId = @event.UsuarioId,
            RecipientId = @event.RecipientId,
            Content = @event.Content,
            SentAt = @event.Timestamp,
            IsRead = false
        };
    }

    // Método para marcar como lida
    public void MarkAsRead()
    {
        IsRead = true;
        ReadAt = DateTime.UtcNow;
    }
}