namespace CodeCafe.Application.Interfaces.Services;
         
public interface IPublicImageBlobService
{
    // Operações básicas
    Task<string> UploadFileAsync(Stream fileStream, string fileName, string? contentType = null);
}