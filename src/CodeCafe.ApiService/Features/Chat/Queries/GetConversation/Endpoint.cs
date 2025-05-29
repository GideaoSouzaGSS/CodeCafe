// Features/Chat/Queries/GetConversation/Endpoint.cs
using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CodeCafe.ApiService.Features.Chat.Queries.GetConversation;

public static class GetConversationEndpoint
{
    public static void MapGetConversationEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/conversations/{user1Id}/{user2Id}", async (
            [FromRoute] int user1Id,
            [FromRoute] int user2Id,
            [FromQuery] int? pageNumber,
            [FromQuery] int? pageSize,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var query = new GetConversationQuery(
                User1Id: user1Id,
                User2Id: user2Id,
                PageNumber: pageNumber ?? 1,
                PageSize: pageSize ?? 20);

            var messages = await mediator.Send(query, cancellationToken);

            return Results.Ok(messages);
        })
        .WithName("GetConversation")
        .WithTags("Chat")
        .Produces<List<MessageDto>>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}