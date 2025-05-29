using MediatR;

namespace CodeCafe.ApiService.Features.Chat.Commands.SendMessage;

public static class SendMessageEndpoint
{
    public static void MapSendMessageEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/messages", async (
            SendMessageRequest request,
            IMediator mediator,
            CancellationToken ct) =>
        {
            var command = new SendMessageCommand(
                request.UsuarioId, 
                request.RecipientId,
                request.Content);
            
            var messageId = await mediator.Send(command, ct);
            return Results.Ok(new { MessageId = messageId });
        })
        .WithName("SendMessage");
    }
}

public record SendMessageRequest(Guid UsuarioId, Guid RecipientId, string Content);