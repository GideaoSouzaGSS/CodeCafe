using MediatR;
using Microsoft.AspNetCore.Mvc;
using CodeCafe.ApiService.Features.Profile.Commands;
using CodeCafe.Application.Interfaces.Services;

namespace CodeCafe.ApiService.Features.Upload.Endpoints;
public static class ImageEndpoints
{
    public static void MapImageEndpoints(this IEndpointRouteBuilder app)
    {
       

        app.MapPost("/api/images/upload", async (IFormFile file, IBlobService blobService, IConfiguration config, HttpContext context) =>
        {
            await context.Request.ReadFormAsync();
            
            if (file == null || file.Length == 0)
            {
                return Results.BadRequest("Nenhum arquivo enviado");
            }

            var containerName = config["BlobStorage:PrivateImagesContainer"] ?? "images";
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

            using var stream = file.OpenReadStream();
            var imageUrl = await blobService.UploadFileAsync(
                stream,
                fileName,
                containerName,
                file.ContentType);

            return Results.Ok(new { Url = imageUrl });

        })
        .WithName("UploadImage")
        .WithTags("Images").DisableAntiforgery();

        app.MapGet("/api/images/{fileName}", async (string fileName, IBlobService blobService, IConfiguration config) =>
        {
            var containerName = config["BlobStorage:ImageContainer"] ?? "images";

            if (!await blobService.FileExistsAsync(fileName, containerName))
            {
                return Results.NotFound();
            }

            var stream = await blobService.DownloadFileAsync(fileName, containerName);
            return Results.File(stream, "image/jpeg"); // Ajuste o content type conforme necessário
        })
        .WithName("GetImage")
        .WithTags("Images");
    }
}