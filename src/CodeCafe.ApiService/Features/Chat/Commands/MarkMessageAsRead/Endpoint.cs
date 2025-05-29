// Features/Chat/Commands/MarkMessageAsRead/Endpoint.cs
using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CodeCafe.ApiService.Features.Chat.Commands.MarkMessageAsRead;
public static class MarkMessageAsReadEndpoint
{
    public static void MapMarkMessageAsReadEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPut("/api/messages/{messageId}/read", async (
            [FromRoute] Guid messageId,
            [FromBody] MarkAsReadRequest request,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var command = new MarkMessageAsReadCommand(
                 messageId,
                request.UserId);

            await mediator.Send(command, cancellationToken);

            return Results.NoContent(); // HTTP 204 No Content
        })
        .WithName("MarkMessageAsRead")
        .WithTags("Chat")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}

public record MarkAsReadRequest(Guid UserId);