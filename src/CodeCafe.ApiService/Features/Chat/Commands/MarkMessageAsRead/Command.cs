using MediatR;

namespace CodeCafe.ApiService.Features.Chat.Commands.MarkMessageAsRead;

public class MarkMessageAsReadCommand : IRequest
{
    public Guid MessageId { get; set; }
    public Guid ReaderId { get; set; }

    public MarkMessageAsReadCommand(Guid messageId, Guid readerId)
    {
        MessageId = messageId;
        ReaderId = readerId;
    }   
    
}