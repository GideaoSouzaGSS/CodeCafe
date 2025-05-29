namespace CodeCafe.Messaging.Messages;

public record PasswordResetRequestedEvent
{
    public Guid UserId { get; init; }
    public string Email { get; init; }
    public string ResetToken { get; init; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}