using CodeCafe.Domain.Entities;

namespace  CodeCafe.Domain.Entities;

public class Conversation
{
    public Guid Id { get; private set; }
    public Guid User1Id { get; private set; }
    public Guid User2Id { get; private set; }
    public DateTime LastMessageAt { get; private set; }
    public ICollection<Message> Messages { get; private set; } = new List<Message>();

    // Construtor privado para EF Core
    private Conversation() { }

    // Factory method
    public static Conversation StartBetween(Guid user1Id, Guid user2Id)
    {
        return new Conversation
        {
            User1Id = user1Id,
            User2Id = user2Id,
            LastMessageAt = DateTime.UtcNow
        };
    }

    public void UpdateLastMessageTime(DateTime timestamp)
    {
        LastMessageAt = timestamp;
    }
}