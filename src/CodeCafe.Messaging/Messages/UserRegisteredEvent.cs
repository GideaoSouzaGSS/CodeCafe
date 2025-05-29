namespace CodeCafe.Messaging.Messages;

public record UserRegisteredEvent
{
    public Guid UserId { get; init; }
    public string Email { get; init; }
    public string Username { get; init; }
    public string ConfirmationToken { get; init; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}
