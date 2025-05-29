namespace CodeCafe.ApiService.Features.Chat.Events.MessageRead;

public record MessageReadEvent(
    Guid MessageId,
    Guid ReaderId) : MessageEvent(Guid.NewGuid(), DateTime.Now);