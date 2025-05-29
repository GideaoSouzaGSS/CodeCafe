namespace CodeCafe.Messaging.Messages;

public record ProfileUpdatedEvent
{
    public Guid UserId { get; init; }
    public string Email { get; init; }
    public string Username { get; init; }
    public DateTime UpdatedAt { get; init; } = DateTime.UtcNow;
}