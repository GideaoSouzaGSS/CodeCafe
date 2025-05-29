using CodeCafe.ApiService.Features.Auth.Commands.TwoFactor;
using MediatR;

namespace CodeCafe.ApiService.Features.Auth.Endpoints;

public static class TwoFactorEndpoints
{
    public static void MapTwoFactorEndpoints(this IEndpointRouteBuilder routes)
    {
        routes.MapPost("/api/auth/habilitar-2fa", async (
            HabilitarTwoFactorCommand request,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var resultado = await mediator.Send(request, cancellationToken);
            return resultado.Success
                ? Results.Ok(new
                {
                    mensagem = "2FA habilitado com sucesso",
                    qrCodeUrl = resultado.Data.QrCodeUrl,
                    tokensRecuperacao = resultado.Data.TokensRecuperacao
                })
                : Results.BadRequest(new { mensagem = resultado.Message });
        }).RequireAuthorization()
        .WithTags("Segundo Fator (2FA)");

        routes.MapPost("/api/auth/desabilitar-2fa", async (
            DesabilitarTwoFactorCommand request,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var resultado = await mediator.Send(request, cancellationToken);
            return resultado.Success
                ? Results.Ok(new { mensagem = "2FA desabilitado com sucesso" })
                : Results.BadRequest(new { mensagem = resultado.Message });
        }).RequireAuthorization()
        .WithTags("Segundo Fator (2FA)");

        routes.MapPost("/api/auth/verificar-2fa", async (
            VerificarCodigoTwoFactorCommand request,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var resultado = await mediator.Send(request, cancellationToken);
            return resultado.Success
                ? Results.Ok(new { token = resultado.Data.Token })
                : Results.BadRequest(new { mensagem = resultado.Message });
        })
        .WithTags("Segundo Fator (2FA)");
    }
}