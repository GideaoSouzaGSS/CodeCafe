namespace CodeCafe.ApiService.Features.Chat.Queries.GetConversation;

public record MessageDto(
    Guid Id,
    int SenderId,
    int RecipientId,
    string Content,
    DateTime SentAt,
    bool IsRead,
    DateTime? ReadAt);