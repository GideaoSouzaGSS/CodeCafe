using MediatR;
using CodeCafe.Domain.Entities;

namespace CodeCafe.ApiService.Features.Albums.Queries;

public record GetAlbumPhotosQuery(Guid AlbumId) : IRequest<IEnumerable<Photo>>;