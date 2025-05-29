using MediatR;
using CodeCafe.ApiService.Features.Albums.Commands;
using CodeCafe.Application.Interfaces.Services;
using CodeCafe.Data;
using CodeCafe.Domain.Entities;

namespace CodeCafe.ApiService.Features.Albums.Handlers;

public class AddPhotoToAlbumHandler : IRequestHandler<AddPhotoToAlbumCommand, string>  
{ 
    private readonly AppDbContext _context; 
    private readonly IPrivateImageBlobService _blobService;

    public AddPhotoToAlbumHandler(AppDbContext context, IPrivateImageBlobService blobService)
    {
        _context = context;
        _blobService = blobService;
    }

    public async Task<string> Handle(AddPhotoToAlbumCommand request, CancellationToken cancellationToken)
    {
        // Verify if album exists
        var album = await _context.Albums.FindAsync(
            new object[] { request.AlbumId }, 
            cancellationToken);

        if (album == null)
            throw new KeyNotFoundException($"Album {request.AlbumId} not found");

        // Generate unique filename to prevent collisions
        var uniqueFileName = $"{request.AlbumId}/{Guid.NewGuid()}{Path.GetExtension(request.FileName)}";

        // Upload file to blob storage
        var blobUrl = await _blobService.UploadFileAsync(
            request.FileStream,
            uniqueFileName,
            request.ContentType);

        // Create photo entity
        var photo = new Photo
        {
            Id = Guid.NewGuid(),
            AlbumId = request.AlbumId,
            Url = uniqueFileName // Store the blob path
        };

        // Save to database
        _context.Photos.Add(photo);
        await _context.SaveChangesAsync(cancellationToken);

        return blobUrl;
    }
}