using MediatR;

namespace CodeCafe.ApiService.Features.Profile.Commands;

public record UpdateProfilePictureCommand(
    Guid UserId,
    IFormFile File) : IRequest<string>;  // Retorna a URL da imagem