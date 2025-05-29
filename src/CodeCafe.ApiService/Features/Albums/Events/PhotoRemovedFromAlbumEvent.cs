namespace CodeCafe.ApiService.Features.Albums.Events;

public record PhotoRemovedFromAlbumEvent(Guid AlbumId, Guid PhotoId);