using MediatR;
using CodeCafe.ApiService.Features.Albums.Events;

namespace CodeCafe.ApiService.Features.Albums.Commands;

public record CreateAlbumCommand(string Name): IRequest<AlbumCreatedEvent>;