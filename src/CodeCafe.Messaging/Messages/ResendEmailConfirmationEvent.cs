namespace CodeCafe.Messaging.Messages;

public record ResendEmailConfirmationEvent
{
    public Guid UserId { get; init; }
    public string Email { get; init; }
    public string Username { get; init; }
    public string ConfirmationToken { get; init; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}
