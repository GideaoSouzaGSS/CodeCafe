using MediatR;

namespace CodeCafe.ApiService.Features.Profile.Events;

public class ProfilePictureUpdatedEvent : INotification
{
    public Guid UsuarioId { get; }
    public string ImageUrl { get; }
    public DateTime UpdatedAt { get; }

    public ProfilePictureUpdatedEvent(Guid usuarioId, string imageUrl)
    {
        UsuarioId = usuarioId;
        ImageUrl = imageUrl;
        UpdatedAt = DateTime.UtcNow;
    }
}