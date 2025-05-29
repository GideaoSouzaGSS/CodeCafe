using MediatR;

namespace CodeCafe.ApiService.Features.Profile.Events;

public class ProfileCoverPictureUpdatedEvent : INotification
{
    public Guid UsuarioId { get; }
    public string CoverImageUrl { get; }
    public DateTime UpdatedAt { get; }

    public ProfileCoverPictureUpdatedEvent(Guid usuarioId, string coverImageUrl)
    {
        UsuarioId = usuarioId;
        CoverImageUrl = coverImageUrl;
        UpdatedAt = DateTime.UtcNow;
    }
}