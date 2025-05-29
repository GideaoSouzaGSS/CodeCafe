using MediatR;
using Microsoft.AspNetCore.Mvc;
using CodeCafe.ApiService.Features.Auth.Queries;
using CodeCafe.ApiService.Features.Auth.Commands.Login;

namespace CodeCafe.ApiService.Features.Auth.Endpoints;
public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/login", async (
            [FromServices] IMediator mediator,
            [FromBody] LoginCommand command
        ) => await mediator.Send(command))
        .WithTags("Autenticação");

        app.MapPost("/api/auth/register", async (
            [FromServices] IMediator mediator,
            [FromBody] RegisterCommand command
        ) => await mediator.Send(command))
        .WithTags("Autenticação");

        app.MapGet("/api/auth/usuarios/{id}", async (Guid id, IMediator mediator) =>
        {
            var usuario = await mediator.Send(new GetUsuarioQuery(id));
            return usuario is null ? Results.NotFound() : Results.Ok(usuario);
        })
        .WithTags("Autenticação");
    }
}