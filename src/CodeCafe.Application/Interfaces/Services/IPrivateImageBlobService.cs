namespace CodeCafe.Application.Interfaces.Services;

public interface IPrivateImageBlobService
{
    // Operações básicas
    Task<string> UploadFileAsync(Stream fileStream, string fileName, string? contentType = null);
    Task<(Stream Content, string ContentType)> DownloadFileAsync(string fileName);
    Task DeleteFileAsync(string fileName);
}