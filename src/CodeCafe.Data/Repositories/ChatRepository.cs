using Microsoft.EntityFrameworkCore;
using CodeCafe.Data;
using CodeCafe.Domain.Entities;
using CodeCafe.Domain.Interfaces;

public class ChatRepository : IChatRepository
{ 
    private readonly AppDbContext _context;

    public ChatRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Conversation?> GetConversationAsync(Guid user1Id, Guid user2Id)
    {
        return await _context.Conversations
            .FirstOrDefaultAsync(c =>
                (c.User1Id == user1Id && c.User2Id == user2Id) ||
                (c.User1Id == user2Id && c.User2Id == user1Id));
    }

    public async Task AddMessageAsync(Message message)
    {
        await _context.Messages.AddAsync(message);
        await _context.SaveChangesAsync();
    }

}