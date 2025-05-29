using CodeCafe.Domain.Entities;

namespace CodeCafe.Domain.Interfaces;
public interface IChatRepository
{
    Task<Conversation?> GetConversationAsync(Guid user1Id, Guid user2Id);
    // Task<Message?> GetMessageAsync(int messageId);
    Task AddMessageAsync(Message message);
    // Task UpdateMessageAsync(Message message);
    // Task<List<Message>> GetConversationMessagesAsync(int conversationId, int page, int pageSize);
}