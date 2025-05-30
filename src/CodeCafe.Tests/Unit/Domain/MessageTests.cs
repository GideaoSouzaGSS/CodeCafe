using System;
using Xunit;
using CodeCafe.Domain.Entities;
using CodeCafe.Domain.Events;

public class MessageTests
{
    [Fact]
    public void MarkAsRead_DeveMarcarMensagemComoLida()
    {
        var user1Id = Guid.NewGuid();
        var user2Id = Guid.NewGuid();
        var conversation = Conversation.StartBetween(user1Id, user2Id);

        var @event = new MessageSentEvent(
            Guid.NewGuid(),
            user1Id,
            user2Id,
            "Olá"
        );
        var message = Message.CreateFromEvent(@event, conversation);
        Assert.False(message.IsRead);
        message.MarkAsRead();
        Assert.True(message.IsRead);
        Assert.NotNull(message.ReadAt);
    }
}