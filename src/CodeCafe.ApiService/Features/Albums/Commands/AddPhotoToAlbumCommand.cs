using MediatR;

namespace CodeCafe.ApiService.Features.Albums.Commands;

public record AddPhotoToAlbumCommand : IRequest<string>
{
    public Guid AlbumId { get; init; }
    public string FileName { get; init; }
    public Stream FileStream { get; init; }
    public string ContentType { get; init; }

    public AddPhotoToAlbumCommand(Guid albumId, string fileName, Stream fileStream, string contentType)
    {
        AlbumId = albumId;
        FileName = fileName;
        FileStream = fileStream;
        ContentType = contentType;
    }
}