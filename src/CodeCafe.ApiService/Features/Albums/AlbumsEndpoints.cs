using MediatR;
using CodeCafe.ApiService.Features.Albums.Commands;
using CodeCafe.ApiService.Features.Albums.Queries;
using CodeCafe.Application.Interfaces.Services;

namespace CodeCafe.ApiService.Features.Albums;

public static class AlbumsEndpoints
{
    public static void MapAlbumsEndpoints(this IEndpointRouteBuilder app)
    {
        #region Albums
        app.MapPost("/api/albums", async (IMediator mediator, CreateAlbumCommand command) =>
        {
            var @event = await mediator.Send(command);
            return Results.Ok(@event);
        })
        .WithName("Albums")
        .WithTags("Images")
        .DisableAntiforgery()
        .RequireAuthorization();

        app.MapGet("/api/albums/{albumId}", async (IMediator mediator, Guid albumId) =>
        {
            var query = new GetAlbumQuery(albumId);
            var album = await mediator.Send(query);
            return Results.Ok(album);
        }).RequireAuthorization();

        app.MapGet("/api/albums", async (IMediator mediator, ICurrentUserService user) =>
        {
            var profileId = user.ProfileId;
            if (profileId == Guid.Empty)
                return Results.Unauthorized();

            var query = new GetAllAlbumsQuery(profileId); 
            var albums = await mediator.Send(query);
            return Results.Ok(albums);
        })
        .WithName("GetAllAlbums")
        .WithTags("Images")
        .RequireAuthorization();
        #endregion

        app.MapPost("/api/albums/{albumId}/photos", async (IMediator mediator, Guid albumId, IFormFile file) =>
        {
            if (file == null || file.Length == 0)
                return Results.BadRequest("File is required.");

            using var stream = file.OpenReadStream();
            var command = new AddPhotoToAlbumCommand(albumId, file.FileName, stream, file.ContentType);
            var photo = await mediator.Send(command);
            return Results.Ok(photo);
        })
        .WithName("AddPhotos")
        .WithTags("Images")
        .DisableAntiforgery()  // Important for file uploads
        .Accepts<IFormFile>("multipart/form-data")  // Specify that we accept multipart/form-data
        .RequireAuthorization();

        app.MapGet("/api/albums/{albumId}/photos", async (IMediator mediator, Guid albumId) =>
        {
            var query = new GetAlbumPhotosQuery(albumId);
            var photos = await mediator.Send(query);
            return Results.Ok(photos);
        })
        .WithName("GetAlbumPhotos")
        .WithTags("Images")
        .RequireAuthorization();

        app.MapDelete("/api/albums/{albumId}/photos/{photoId}", async (IMediator mediator, Guid albumId, Guid photoId) =>
        {
            var command = new RemovePhotoFromAlbumCommand(albumId, photoId);
            var @event = await mediator.Send(command);
            return Results.Ok(@event);
        })
        .WithName("DeletePhotoFromAlbum")
        .WithTags("Images")
        .RequireAuthorization();

        app.MapGet("/api/photos/{**fileName}", async (IPrivateImageBlobService blobService, ILogger<Program> logger, string fileName) =>
        {
            try
            {
                // Decode the URL-encoded filename
                var decodedFileName = Uri.UnescapeDataString(fileName);
                logger.LogInformation("Attempting to download file: {FileName}", decodedFileName);

                var (content, contentType) = await blobService.DownloadFileAsync(decodedFileName);
                
                if (content == null)
                {
                    logger.LogWarning("File not found: {FileName}", decodedFileName);
                    return Results.NotFound(new { message = $"File {decodedFileName} not found" });
                }

                logger.LogInformation("Successfully retrieved file: {FileName}", decodedFileName);
                return Results.File(content, contentType);
            }
            catch (FileNotFoundException ex)
            {
                logger.LogError(ex, "File not found: {FileName}", fileName);
                return Results.NotFound(new { message = $"File {fileName} not found in container private-images" });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving file: {FileName}", fileName);
                return Results.StatusCode(500);
            }
        })
        .WithName("GetPhoto")
        .WithTags("Images")
        .RequireAuthorization();
    }
}