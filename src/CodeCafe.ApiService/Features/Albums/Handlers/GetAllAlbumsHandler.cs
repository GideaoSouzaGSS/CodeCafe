using MediatR;
using Microsoft.EntityFrameworkCore;
using CodeCafe.ApiService.Features.Albums.Queries;
using CodeCafe.Domain.Entities;
using CodeCafe.Data;

namespace CodeCafe.ApiService.Features.Albums.Handlers;

public class GetAllAlbumsHandler : IRequestHandler<GetAllAlbumsQuery, IEnumerable<Album>>
{
    private readonly AppDbContext _context;

    public GetAllAlbumsHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Album>> Handle(GetAllAlbumsQuery request, CancellationToken cancellationToken)
    {
        return await _context.Albums
            .Where(a => a.UserProfileId == request.profileId)
            .ToListAsync(cancellationToken);
    }
}