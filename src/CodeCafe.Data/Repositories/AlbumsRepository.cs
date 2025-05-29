using Microsoft.EntityFrameworkCore;
using CodeCafe.Domain.Entities;
using CodeCafe.Domain.Interfaces;

namespace CodeCafe.Data.Repositories;
public class AlbumsRepository : IAlbumsRepository
{
    private readonly AppDbContext _context;

    public AlbumsRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Album> AddAlbumAsync(Album album)
    {
        _context.Set<Album>().Add(album);
        await _context.SaveChangesAsync();
        return album;
    }

    public async Task<bool> DeleteAlbumAsync(Guid id)
    {
        var album = await _context.Set<Album>().FindAsync(id);
        if (album == null)
        {
            return false;
        }

        _context.Set<Album>().Remove(album);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Album> GetAlbumByIdAsync(Guid id)
    {
        var album = await _context.Set<Album>().FindAsync(id);
        if (album == null)
        {
            throw new KeyNotFoundException($"Album with ID {id} was not found.");
        }
        return album;
    }

    public async Task<List<Album>> GetAllAlbumsAsync()
    {
        return await _context.Set<Album>().ToListAsync();
    }

    public async Task<bool> UpdateAlbumAsync(Album album)
    {
        var existingAlbum = await _context.Set<Album>().FindAsync(album.Id);
        if (existingAlbum == null)
        {
            return false;
        }

        _context.Entry(existingAlbum).CurrentValues.SetValues(album);
        await _context.SaveChangesAsync();
        return true;
    }
}