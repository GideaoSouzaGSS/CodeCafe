using MediatR;
using CodeCafe.Domain.Entities;

namespace CodeCafe.ApiService.Features.Albums.Queries;

public record GetAllAlbumsQuery(Guid profileId) : IRequest<IEnumerable<Album>>;