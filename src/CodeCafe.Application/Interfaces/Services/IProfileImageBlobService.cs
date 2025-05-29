namespace CodeCafe.Application.Interfaces.Services;
         
public interface IProfileImageBlobService
{
    // Operações básicas
    Task<string> UploadFileAsync(Stream fileStream, string fileName, string? contentType = null);
}