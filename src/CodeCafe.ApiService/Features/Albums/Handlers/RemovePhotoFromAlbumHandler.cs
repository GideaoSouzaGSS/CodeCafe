using MediatR;
using Microsoft.EntityFrameworkCore;
using CodeCafe.ApiService.Features.Albums.Commands;
using CodeCafe.Application.Interfaces.Services;
using CodeCafe.Data;

namespace CodeCafe.ApiService.Features.Albums.Handlers;

public class RemovePhotoFromAlbumHandler : IRequestHandler<RemovePhotoFromAlbumCommand, bool>
{
    private readonly AppDbContext _context;
    private readonly IPrivateImageBlobService _blobService;

    public RemovePhotoFromAlbumHandler(AppDbContext context, IPrivateImageBlobService blobService)
    {
        _context = context;
        _blobService = blobService;
    }

    public async Task<bool> Handle(RemovePhotoFromAlbumCommand request, CancellationToken cancellationToken)
    {
        var photo = await _context.Photos
            .FirstOrDefaultAsync(p => p.Id == request.PhotoId && p.AlbumId == request.AlbumId, 
                cancellationToken);

        if (photo == null)
            return false;

        // Delete from blob storage
        await _blobService.DeleteFileAsync(photo.Url);

        // Delete from database
        _context.Photos.Remove(photo);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}