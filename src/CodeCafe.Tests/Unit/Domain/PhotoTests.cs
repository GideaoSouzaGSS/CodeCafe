using System;
using Xunit;
using CodeCafe.Domain.Entities;

public class PhotoTests
{
    [Fact]
    public void CriarPhoto_DeveSetarPropriedades()
    {
        var albumId = Guid.NewGuid();
        var photo = new Photo
        {
            Id = Guid.NewGuid(),
            Url = "https://site.com/foto.jpg",
            AlbumId = albumId
        };

        Assert.Equal("https://site.com/foto.jpg", photo.Url);
        Assert.Equal(albumId, photo.AlbumId);
        Assert.NotEqual(Guid.Empty, photo.Id);
    }
}