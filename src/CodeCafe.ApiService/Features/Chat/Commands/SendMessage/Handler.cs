using System.Text.Json;
using MediatR;
using Microsoft.EntityFrameworkCore;
using CodeCafe.Data;
using CodeCafe.Data.Store;
using CodeCafe.Domain.Entities;
using CodeCafe.Domain.Interfaces;
using CodeCafe.Domain.Events;

namespace CodeCafe.ApiService.Features.Chat.Commands.SendMessage;

public class SendMessageHandler : IRequestHandler<SendMessageCommand, Guid>
{
    private readonly IEventStore _eventStore;
    private readonly IChatRepository _chatRepository;
    private readonly AppDbContext _appDbContext;
    public SendMessageHandler(IEventStore eventStore, IChatRepository chatRepository, AppDbContext appDbContext)
    {
        _eventStore = eventStore;
        _chatRepository = chatRepository;
          _appDbContext = appDbContext;
    }
    public async Task<Guid> Handle(SendMessageCommand command, CancellationToken ct)
    {

                // 1. Criar evento
        var messageId = Guid.NewGuid();
        var @event = new MessageSentEvent(
            messageId,
            command.UsuarioId,
            command.RecipientId,
            command.Content);

        // 2. Persistir evento
        await _eventStore.SaveEventAsync(@event);
        // 3. Atualizar read model
        var conversation = await _appDbContext.Conversations
            .FirstOrDefaultAsync(c =>
                (c.User1Id == command.UsuarioId && c.User2Id == command.RecipientId) ||
                (c.User1Id == command.RecipientId && c.User2Id == command.UsuarioId), ct);

        if (conversation == null)
        {
            conversation = Conversation.StartBetween(command.UsuarioId, command.RecipientId);
            _appDbContext.Conversations.Add(conversation);
        }

        var message = Message.CreateFromEvent(@event, conversation);
        _appDbContext.Messages.Add(message);

        // 4. Salvar mudanças
        await _appDbContext.SaveChangesAsync(ct);
        
        // 5. Publicar evento
        // await _messageBus.PublishAsync(@event);
        
        return messageId;
    }
}