using MediatR;

namespace CodeCafe.ApiService.Features.Chat.Queries.GetConversation;

public record GetConversationQuery(
    int User1Id,
    int User2Id,
    int PageNumber = 1,
    int PageSize = 20) : IRequest<List<MessageDto>>;