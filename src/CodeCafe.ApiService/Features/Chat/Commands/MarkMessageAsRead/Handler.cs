// Features/Chat/Commands/MarkMessageAsRead/Handler.cs
using MediatR;
using Microsoft.EntityFrameworkCore;
using CodeCafe.ApiService.Features.Chat.Events.MessageRead;

namespace CodeCafe.ApiService.Features.Chat.Commands.MarkMessageAsRead;

public class MarkMessageAsReadHandler : IRequestHandler<MarkMessageAsReadCommand>
{
    private readonly CodeCafe.Data.AppDbContext _context;

    public MarkMessageAsReadHandler(CodeCafe.Data.AppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(MarkMessageAsReadCommand command, CancellationToken ct)
    {
        var message = await _context.Messages
            .FirstOrDefaultAsync(m => m.Id == command.MessageId, ct);

        if (message != null && message.RecipientId == command.ReaderId)
        {
            var @event = new MessageReadEvent( 
                command.MessageId,
                command.ReaderId);  

            message.MarkAsRead();
            await _context.SaveChangesAsync(ct);
            // await _messageBus.PublishAsync(@event);
        }
    }
}