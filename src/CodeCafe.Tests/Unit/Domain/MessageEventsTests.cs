using System;
using CodeCafe.Domain.Events;
using Xunit;

public class MessageEventsTests
{
    [Fact]
    public void CriarMessageSentEvent_DeveSetarPropriedadesCorretamente()
    {
        var messageId = Guid.NewGuid();
        var usuarioId = Guid.NewGuid();
        var recipientId = Guid.NewGuid();
        var content = "Olá, mundo!";

        var evt = new MessageSentEvent(messageId, usuarioId, recipientId, content);

        Assert.Equal(messageId, evt.MessageId);
        Assert.Equal(usuarioId, evt.UsuarioId);
        Assert.Equal(recipientId, evt.RecipientId);
        Assert.Equal(content, evt.Content);
        Assert.NotEqual(default, evt.EventId);
        Assert.True((DateTime.Now - evt.Timestamp).TotalSeconds < 2);
    }
}