using MediatR;
using CodeCafe.ApiService.Features.Albums.Commands;
using CodeCafe.ApiService.Features.Albums.Events;
using CodeCafe.Application.Interfaces.Services;
using CodeCafe.Data;
using CodeCafe.Domain.Entities;

namespace CodeCafe.ApiService.Features.Albums.Handlers;

public class CreateAlbumHandler : IRequestHandler<CreateAlbumCommand, AlbumCreatedEvent>
{
    private readonly IMediator _mediator;
    private readonly AppDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public CreateAlbumHandler(AppDbContext context, ICurrentUserService currentUserService, IMediator mediator)
    {
        _mediator = mediator;
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<AlbumCreatedEvent> Handle(CreateAlbumCommand command, CancellationToken cancellationToken)
    {
        // Obter o ID do usuário atual
        var profileId = _currentUserService.ProfileId;
        if (profileId == null)
            throw new UnauthorizedAccessException("User is not authenticated.");

        // Criar o álbum
        var album = new Album
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            UserProfileId = profileId
        };

        _context.Albums.Add(album);
        await _context.SaveChangesAsync(cancellationToken);
        
        // Retornar o evento de criação do álbum
        var albumCriado = new AlbumCreatedEvent(profileId, album.Id, album.Name, album.UserProfileId);

        await _mediator.Publish(albumCriado, cancellationToken);

        return albumCriado;
    }
}