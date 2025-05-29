using MediatR;
using Microsoft.EntityFrameworkCore;
using CodeCafe.ApiService.Features.Profile.Events;
using CodeCafe.Application.Interfaces.Services;
using CodeCafe.Data;

namespace CodeCafe.ApiService.Features.Profile.Commands;
 
public class UpdateProfilePictureCoverCommandHandler : IRequestHandler<UpdateProfilePictureCoverCommand, string>
{ 
    private readonly IProfileImageBlobService _profileBlobService; 
    private readonly AppDbContext _context;
    private readonly ILogger<UpdateProfilePictureCoverCommandHandler> _logger;
    private readonly IMediator _mediator;

    public UpdateProfilePictureCoverCommandHandler(
        IProfileImageBlobService blobService,
        AppDbContext context,
        ILogger<UpdateProfilePictureCoverCommandHandler> logger,
        IMediator mediator)
    {
        _profileBlobService = blobService;
        _context = context;
        _logger = logger;
        _mediator = mediator;
    }

    public async Task<string> Handle(UpdateProfilePictureCoverCommand command, CancellationToken ct)
    {
        try
        {
            // 1. Validar arquivo
            if (command.File == null || command.File.Length == 0)
                throw new Exception("Nenhum arquivo enviado");

            if (command.File.Length > 5 * 1024 * 1024) // 5MB
                throw new Exception("Tamanho máximo excedido (5MB)");

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(command.File.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(extension))
                throw new Exception("Formato de imagem não suportado");

            // 2. Upload para Blob Storage

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(command.File.FileName)}";

            using var stream = command.File.OpenReadStream();
            var imageUrl = await _profileBlobService.UploadFileAsync(
                stream,
                fileName,
                command.File.ContentType);

            // 3. Atualizar perfil do usuário
            var user = await _context.UserProfiles
                .FirstOrDefaultAsync(u => u.UsuarioId == command.UserId, ct);

            if (user == null)
                throw new Exception("Usuário não encontrado");

            user.UpdateImageCoverProfile(imageUrl);
            await _context.SaveChangesAsync(ct);

            // Publish event
            var profileCoverUpdatedEvent = new ProfileCoverPictureUpdatedEvent(command.UserId, imageUrl);
            await _mediator.Publish(profileCoverUpdatedEvent, ct);

            return imageUrl;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar foto de capa");
            throw;
        }
    }
}
