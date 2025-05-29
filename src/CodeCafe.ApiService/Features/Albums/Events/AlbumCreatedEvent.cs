using MediatR;

namespace CodeCafe.ApiService.Features.Albums.Events;

public record AlbumCreatedEvent( Guid UsuarioId, Guid AlbumId, string Name, Guid UserProfileId) : INotification;