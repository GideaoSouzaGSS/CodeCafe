namespace CodeCafe.Domain.Entities;

public class UserProfile
{
    public Guid Id { get; private set; }
    public string Username { get; private set; }
    public string DisplayName { get; private set; }
    public string Bio { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Guid UsuarioId { get; private set; }
    public string? PhotoUrl { get; set; }
    public string? CoverPhotoUrl { get; set; }

    public bool AccepptFollow { get; set; }
    public bool AcceptDirectMessage { get; set; }

    // Relacionamentos
    private readonly List<UserFollowing> _followings = new();
    private readonly List<UserFollowing> _followers = new();
    private readonly List<Album> _albums = new();

    public IReadOnlyCollection<UserFollowing> Followings => _followings.AsReadOnly();
    public IReadOnlyCollection<UserFollowing> Followers => _followers.AsReadOnly();
    public IReadOnlyCollection<Album> Albums => _albums.AsReadOnly();

    protected UserProfile() { } // Para o EF Core

    // Factory Method
    public static UserProfile Create(
        Guid usuarioId,
        string username,
        string displayName,
        string bio,
        bool acceptFollow,
        bool acceptDirectMessage)
    {
        var profile = new UserProfile
        {
            Id = Guid.NewGuid(),
            UsuarioId = usuarioId,
            Username = username,
            DisplayName = displayName,
            Bio = bio,
            AccepptFollow = acceptFollow,
            AcceptDirectMessage = acceptDirectMessage,
            CreatedAt = DateTime.UtcNow
        };

        return profile;
    }

    public void UpdateProfile(string username, string displayName, string bio, bool acceptFollow, bool acceptDirectMessage)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username cannot be empty.", nameof(username));

        if (string.IsNullOrWhiteSpace(displayName))
            throw new ArgumentException("Display name cannot be empty.", nameof(displayName));

        // Atualiza os campos do perfil
        Username = username;
        DisplayName = displayName;
        Bio = bio;
        AccepptFollow = acceptFollow;
        AcceptDirectMessage = acceptDirectMessage;

        // Adicione lógica adicional, se necessário, como validações ou eventos de domínio
    }
    public void UpdateImageProfile(string photoUrl)
    {
        PhotoUrl = photoUrl;
    }

    public void UpdateImageCoverProfile(string photoUrl)
    {
        CoverPhotoUrl = photoUrl;
    }
    public UserProfile(Guid usuarioId, Guid id, string username, string displayName)
    {
        Id = id;
        UsuarioId = usuarioId;
        Username = username;
        DisplayName = displayName;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateProfile(string displayName, string bio)
    {
        DisplayName = displayName;
        Bio = bio;
    }

    public void Follow(UserProfile userToFollow)
    {
        if (_followings.Any(f => f.FollowedId == userToFollow.Id))
            return;

        _followings.Add(new UserFollowing(this, userToFollow));
    }

    public void Unfollow(UserProfile userToUnfollow)
    {
        var following = _followings.FirstOrDefault(f => f.FollowedId == userToUnfollow.Id);
        if (following != null)
        {
            _followings.Remove(following);
        }
    }
}