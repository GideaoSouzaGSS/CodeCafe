using MediatR;
using CodeCafe.ApiService.Features.Follow.Commands;
using CodeCafe.ApiService.Features.Profile.Commands;
using CodeCafe.ApiService.Features.Profile.Queries;
using CodeCafe.Application.Interfaces.Services;

namespace CodeCafe.ApiService.Features.Follow;
public static class ProfileEndpoints
{
    public static void MapProfileEndpoints(this IEndpointRouteBuilder app)
    {

        app.MapGet("/api/profiles/me", async (IMediator mediator, ICurrentUserService currentUser) =>
        {
            var query = new GetUserProfileQuery(currentUser.ProfileId);
            var profile = await mediator.Send(query);
            return Results.Ok(profile);
        }).RequireAuthorization();

        app.MapGet("api/profiles/{userId}", async (
            Guid userId,
            IMediator mediator,
            CancellationToken ct) =>
        {
            var query = new GetUserProfileQuery(userId);
            var profile = await mediator.Send(query, ct);
            return Results.Ok(profile);
        });

        app.MapPost("api/profiles/{userId}/follow", async (
            Guid userId,
            IMediator mediator,
            ICurrentUserService currentUser,
            CancellationToken ct) =>
        {
            var command = new FollowUserCommand(currentUser.ProfileId, userId);
            await mediator.Send(command, ct);
            return Results.NoContent();
        });

        app.MapDelete("api/profiles/{userId}/follow", async (
            Guid userId,
            IMediator mediator,
            ICurrentUserService currentUser,
            CancellationToken ct) =>
        {
            var command = new UnfollowUserCommand(currentUser.ProfileId, userId);
            await mediator.Send(command, ct);
            return Results.NoContent();
        });

        app.MapPost("/api/profile/photo", async (IFormFile file, IBlobService blobService,
        IConfiguration config, HttpContext context, IMediator mediator, ICurrentUserService currentUser) =>
        {
            if (file == null || file.Length == 0)
            {
                return Results.BadRequest("Nenhum arquivo enviado");
            }

            var command = new UpdateProfilePictureCommand(currentUser.UserId, file);
            var imageUrl = await mediator.Send(command);
            return imageUrl != null ? Results.Ok(new { Url = imageUrl }) : Results.BadRequest("Erro ao atualizar imagem de perfil");
        })
        .WithName("UploadProfilePhoto")
        .WithTags("Images").DisableAntiforgery();

        app.MapPost("/api/profile/cover", async (IFormFile file, IMediator mediator, ICurrentUserService currentUser) =>
        {
            if (file == null || file.Length == 0)
            {
                return Results.BadRequest("Nenhum arquivo enviado");
            }

            var command = new UpdateProfilePictureCoverCommand(currentUser.UserId, file);
            var imageUrl = await mediator.Send(command);

            return imageUrl != null
                ? Results.Ok(new { Url = imageUrl })
                : Results.BadRequest("Erro ao atualizar imagem de capa");
        })
        .WithName("UploadCoverPhoto")
        .WithTags("Profile").DisableAntiforgery(); // Desabilita CSRF para APIs stateless

        app.MapPut("/api/profile/update", async (UpdateProfileRequestDto requestDto, IMediator mediator, ICurrentUserService currentUserService) =>
        {
            if (currentUserService.UserId == Guid.Empty) // Ou a verificação de autenticação apropriada
            {
                return Results.Unauthorized();
            }

            var command = new UpdateProfileCommand(
                currentUserService.UserId,
                requestDto.Username,
                requestDto.DisplayName,
                requestDto.Bio,
                requestDto.AcceptFollow,
                requestDto.AcceptDirectMessage
            );

            var success = await mediator.Send(command);

            return success ? Results.Ok("Perfil atualizado com sucesso.") : Results.BadRequest("Erro ao atualizar o perfil.");
        })
        .WithName("UpdateUserProfile")
        .WithTags("Profile")
        .RequireAuthorization();
    }
}
