using MediatR;
using CodeCafe.Domain.Entities;

namespace CodeCafe.ApiService.Features.Albums.Queries;

public record GetAlbumQuery(Guid AlbumId) : IRequest<Album>;