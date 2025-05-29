using CodeCafe.Domain.Entities;

namespace CodeCafe.Domain.Interfaces;
public interface IAlbumsRepository
{
    Task<Album> GetAlbumByIdAsync(Guid id);
    Task<List<Album>> GetAllAlbumsAsync();
    Task<Album> AddAlbumAsync(Album album);
    Task<bool> UpdateAlbumAsync(Album album);
    Task<bool> DeleteAlbumAsync(Guid id);
}