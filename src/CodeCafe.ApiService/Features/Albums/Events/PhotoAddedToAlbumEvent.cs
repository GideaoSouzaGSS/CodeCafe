namespace CodeCafe.ApiService.Features.Albums.Events;

public record PhotoAddedToAlbumEvent(Guid AlbumId, Guid PhotoId, string PhotoUrl);