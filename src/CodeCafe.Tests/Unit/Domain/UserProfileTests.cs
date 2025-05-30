using System;
using Xunit;
using CodeCafe.Domain.Entities;

public class UserProfileTests
{
    [Fact]
    public void UpdateProfile_ComDadosValidos_AtualizaCampos()
    {
        var profile = UserProfile.Create(
            Guid.NewGuid(),
            "user",
            "User Name",
            "bio",
            true,
            false);

        profile.UpdateProfile("user2", "User Name 2", "bio2", false, true);

        Assert.Equal("user2", profile.Username);
        Assert.Equal("User Name 2", profile.DisplayName);
        Assert.Equal("bio2", profile.Bio);
        Assert.False(profile.AccepptFollow);
        Assert.True(profile.AcceptDirectMessage);
    }

    [Fact]
    public void UpdateProfile_ComUsernameVazio_DeveLancarExcecao()
    {
        var profile = UserProfile.Create(
            Guid.NewGuid(),
            "user",
            "User Name",
            "bio",
            true,
            false);

        Assert.Throws<ArgumentException>(() => profile.UpdateProfile("", "Display", "bio", true, true));
    }

    [Fact]
    public void UpdateImageProfile_AtualizaPhotoUrl()
    {
        var profile = UserProfile.Create(
            Guid.NewGuid(),
            "user",
            "User Name",
            "bio",
            true,
            false);

        profile.UpdateImageProfile("http://img.com/foto.png");
        Assert.Equal("http://img.com/foto.png", profile.PhotoUrl);
    }

    [Fact]
    public void UpdateImageCoverProfile_AtualizaCoverPhotoUrl()
    {
        var profile = UserProfile.Create(
            Guid.NewGuid(),
            "user",
            "User Name",
            "bio",
            true,
            false);

        profile.UpdateImageCoverProfile("http://img.com/capa.png");
        Assert.Equal("http://img.com/capa.png", profile.CoverPhotoUrl);
    }
}