namespace CodeCafe.Domain.Entities;
public class UserFollowing
{
    public Guid FollowerId { get; private set; }
    public UserProfile Follower { get; private set; }
    
    public Guid FollowedId { get; private set; }
    public UserProfile Followed { get; private set; }
    
    public DateTime FollowedAt { get; private set; }

    protected UserFollowing() { } // Para o EF Core

    public UserFollowing(UserProfile follower, UserProfile followed)
    {
        Follower = follower;
        FollowerId = follower.Id;
        Followed = followed;
        FollowedId = followed.Id;
        FollowedAt = DateTime.UtcNow;
    }
}