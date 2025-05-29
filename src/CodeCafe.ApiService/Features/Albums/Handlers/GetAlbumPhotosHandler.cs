using MediatR;
using Microsoft.EntityFrameworkCore;
using CodeCafe.ApiService.Features.Albums.Queries;
using CodeCafe.Domain.Entities;
using CodeCafe.Data;

namespace CodeCafe.ApiService.Features.Albums.Handlers;

public class GetAlbumPhotosHandler : IRequestHandler<GetAlbumPhotosQuery, IEnumerable<Photo>>
{
    private readonly AppDbContext _context;

    public GetAlbumPhotosHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Photo>> Handle(GetAlbumPhotosQuery request, CancellationToken cancellationToken)
    {
        var photos = await _context.Photos
            .Where(p => p.AlbumId == request.AlbumId)
            .ToListAsync(cancellationToken);

        return photos;
    }
}