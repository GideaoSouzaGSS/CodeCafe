using MediatR;

namespace CodeCafe.ApiService.Features.Profile.Events;

public class ProfileUpdatedEvent : INotification
{
    public Guid UsuarioId { get; set; }
    public string Username { get; set; }
    public string DisplayName { get; set; }
    public string Bio { get; set; }
    public bool AcceptFollow { get; set; }
    public bool AcceptDirectMessage { get; set; }
    public DateTime UpdatedAt { get; set; }

    public ProfileUpdatedEvent(
        Guid usuarioId,
        string username,
        string displayName,
        string bio,
        bool acceptFollow,
        bool acceptDirectMessage)
    {
        UsuarioId = usuarioId;
        Username = username;
        DisplayName = displayName;
        Bio = bio;
        AcceptFollow = acceptFollow;
        AcceptDirectMessage = acceptDirectMessage;
        UpdatedAt = DateTime.UtcNow;
    }
}