using System;
using Xunit;
using CodeCafe.Domain.Entities;

public class AlbumTests
{
    [Fact]
    public void CriarAlbum_DeveSetarPropriedades()
    {
        var userId = Guid.NewGuid();
        var album = new Album
        {
            Id = Guid.NewGuid(),
            Name = "Viagem",
            UserProfileId = userId
        };

        Assert.Equal("Viagem", album.Name);
        Assert.Equal(userId, album.UserProfileId);
        Assert.Empty(album.Photos);
    }

    [Fact]
    public void AdicionarFoto_DeveAdicionarNaLista()
    {
        var album = new Album { Name = "Viagem" };
        var photo = new Photo(); // Supondo que exista uma classe Photo

        album.Photos.Add(photo);

        Assert.Single(album.Photos);
        Assert.Contains(photo, album.Photos);
    }
}