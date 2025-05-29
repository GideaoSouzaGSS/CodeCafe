using MediatR;

namespace CodeCafe.ApiService.Features.EventStoreHistory;
public static class EventEndpoints
{
    public static void EventMapEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/events/", async (IMediator mediator) =>
        {
            var query = new GetEventsQuery();
            var eventos = await mediator.Send(query);
            return Results.Ok(eventos);
        });
    }
}