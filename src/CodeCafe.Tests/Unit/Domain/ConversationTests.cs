using System;
using Xunit;
using CodeCafe.Domain.Entities;

public class ConversationTests
{
    [Fact]
    public void StartBetween_DeveCriarConversationComUsuariosCorretos()
    {
        var user1Id = Guid.NewGuid();
        var user2Id = Guid.NewGuid();

        var conversation = Conversation.StartBetween(user1Id, user2Id);

        Assert.Equal(user1Id, conversation.User1Id);
        Assert.Equal(user2Id, conversation.User2Id);
        Assert.True((DateTime.UtcNow - conversation.LastMessageAt).TotalSeconds < 2);
        Assert.Empty(conversation.Messages);
    }

    [Fact]
    public void UpdateLastMessageTime_DeveAtualizarLastMessageAt()
    {
        var user1Id = Guid.NewGuid();
        var user2Id = Guid.NewGuid();
        var conversation = Conversation.StartBetween(user1Id, user2Id);

        var newTime = DateTime.UtcNow.AddMinutes(5);
        conversation.UpdateLastMessageTime(newTime);

        Assert.Equal(newTime, conversation.LastMessageAt);
    }
}