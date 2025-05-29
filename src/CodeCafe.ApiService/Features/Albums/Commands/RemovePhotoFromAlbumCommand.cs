using MediatR;

namespace CodeCafe.ApiService.Features.Albums.Commands;

public record RemovePhotoFromAlbumCommand : IRequest<bool>
{
    public Guid AlbumId { get; init; }
    public Guid PhotoId { get; init; }

    public RemovePhotoFromAlbumCommand(Guid albumId, Guid photoId)
    {
        AlbumId = albumId;
        PhotoId = photoId;
    }
}