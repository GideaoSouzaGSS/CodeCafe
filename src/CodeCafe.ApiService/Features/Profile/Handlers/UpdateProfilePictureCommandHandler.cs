using MediatR;
using Microsoft.EntityFrameworkCore;
using CodeCafe.ApiService.Features.Profile.Events;
using CodeCafe.Application.Interfaces.Services;
using CodeCafe.Data;
using FluentValidation; // Adicione este using
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Gif;

namespace CodeCafe.ApiService.Features.Profile.Commands;
 
public class UpdateProfilePictureCommandHandler : IRequestHandler<UpdateProfilePictureCommand, string>
{ 
    private readonly IProfileImageBlobService _profileBlobService; 
    private readonly AppDbContext _context;
    private readonly ILogger<UpdateProfilePictureCommandHandler> _logger;
    private readonly IMediator _mediator;

    public UpdateProfilePictureCommandHandler(
        IProfileImageBlobService blobService,
        AppDbContext context,
        ILogger<UpdateProfilePictureCommandHandler> logger,
        IMediator mediator)
    {
        _profileBlobService = blobService;
        _context = context;
        _logger = logger;
        _mediator = mediator;
    }

    public async Task<string> Handle(UpdateProfilePictureCommand command, CancellationToken ct)
    {
        try
        {
            // 1. Validar arquivo (exceções específicas)
            if (command.File == null || command.File.Length == 0)
                throw new ValidationException("Nenhum arquivo enviado");

            if (command.File.Length > 5 * 1024 * 1024) // 5MB
                throw new ValidationException("Tamanho máximo excedido (5MB)");

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(command.File.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(extension))
                throw new ValidationException("Formato de imagem não suportado");

            // Validação extra: checar ContentType
            var allowedContentTypes = new[] { "image/jpeg", "image/png", "image/gif" };
            if (!allowedContentTypes.Contains(command.File.ContentType))
                throw new ValidationException("Tipo de conteúdo não suportado");

            // (Opcional) Validar magic number do arquivo para garantir que é imagem
            // Exemplo para JPEG: FF D8 FF
            using var stream = command.File.OpenReadStream();
            byte[] header = new byte[4];
            await stream.ReadAsync(header, 0, 4, ct);
            stream.Position = 0; // Resetar para upload
            if (extension == ".jpg" || extension == ".jpeg")
            {
                if (!(header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF))
                    throw new ValidationException("Arquivo não é uma imagem JPEG válida");
            }
            // Adicione validações para outros formatos se desejar

            // 2. Upload para Blob Storage
            var fileName = $"{Guid.NewGuid()}{extension}";
            using (var image = await Image.LoadAsync(stream, ct))
            {
                stream.Position = 0;
                using var ms = new MemoryStream();

                // Detecta o formato e salva corretamente
                IImageEncoder encoder = extension switch
                {
                    ".png" => new PngEncoder(),
                    ".gif" => new GifEncoder(),
                    _ => new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder()
                };

                await image.SaveAsync(ms, encoder, ct);
                ms.Position = 0;

                // Upload ms em vez de stream original
                var imageUrl = await _profileBlobService.UploadFileAsync(
                    ms,
                    fileName,
                    command.File.ContentType);

                // 3. Atualizar perfil do usuário
                var user = await _context.UserProfiles
                    .FirstOrDefaultAsync(u => u.UsuarioId == command.UserId, ct);

                if (user == null)
                    throw new Exception("Usuário não encontrado");

                user.UpdateImageProfile(imageUrl);
                await _context.SaveChangesAsync(ct);

                // Publicar evento
                var profilePictureUpdatedEvent = new ProfilePictureUpdatedEvent(command.UserId, imageUrl);
                await _mediator.Publish(profilePictureUpdatedEvent, ct);

                return imageUrl;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar foto de perfil");
            throw;
        }
    }
}
