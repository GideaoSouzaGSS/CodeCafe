using CodeCafe.ApiService.Features.Auth.Commands.EmailConfirmation;
using MediatR;

namespace CodeCafe.ApiService.Features.Auth.Endpoints;

public static class EmailConfirmationEndpoints
{
    public static void MapEmailConfirmationEndpoints(this IEndpointRouteBuilder routes)
    {
        routes.MapPost("/api/auth/solicitar-confirmacao-email", async (
            SolicitarConfirmacaoEmailCommand request, 
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var resultado = await mediator.Send(request, cancellationToken);
            return resultado.Success
                ? Results.Ok(new { mensagem = "Email de confirmação enviado com sucesso" })
                : Results.BadRequest(new { mensagem = resultado.Message });
        }).WithTags("Confirmação de Email");

        routes.MapPost("/api/auth/confirmar-email", async (
            ConfirmarEmailCommand request, 
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var resultado = await mediator.Send(request, cancellationToken);
            return resultado.Success
                ? Results.Ok(new { mensagem = "Email confirmado com sucesso" })
                : Results.BadRequest(new { mensagem = resultado.Message });
        }).WithTags("Confirmação de Email");;
    }
}