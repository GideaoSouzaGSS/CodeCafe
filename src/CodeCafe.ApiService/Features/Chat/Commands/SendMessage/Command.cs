using MediatR;

namespace CodeCafe.ApiService.Features.Chat.Commands.SendMessage;

public record SendMessageCommand(
    Guid UsuarioId,
    Guid RecipientId,
    string Content) : IRequest<Guid>;