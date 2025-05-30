using System;
using Xunit;
using CodeCafe.Domain.Entities;

public class UserFollowingTests
{
    [Fact]
    public void CriarUserFollowing_DeveSetarPropriedades()
    {
        var follower = UserProfile.Create(
            Guid.NewGuid(),
            "followerUser",
            "Follower Name",
            "Bio do follower",
            true,
            true);

        var followed = UserProfile.Create(
            Guid.NewGuid(),
            "followedUser",
            "Followed Name",
            "Bio do followed",
            true,
            true);

        var following = new UserFollowing(follower, followed);

        Assert.Equal(follower, following.Follower);
        Assert.Equal(followed, following.Followed);
        Assert.NotEqual(default, following.FollowedAt);
    }
}