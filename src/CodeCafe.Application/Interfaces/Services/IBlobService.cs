using Azure.Storage.Blobs.Models;
namespace CodeCafe.Application.Interfaces.Services;
public interface IBlobService
{
    // Operações básicas
    Task<string> UploadFileAsync(Stream fileStream, string fileName, string containerName, string? contentType = null);
    Task<Stream> DownloadFileAsync(string fileName, string containerName);
    Task<Stream> DownloadFilePrivateAsync(string fileName);
    
    Task DeleteFileAsync(string fileName, string containerName);
    Task<bool> FileExistsAsync(string fileName, string containerName);
    
    // Operações avançadas
    Task<string> GenerateSasTokenAsync(string fileName, string containerName, TimeSpan expiryTime);
    Task SetMetadataAsync(string fileName, string containerName, IDictionary<string, string> metadata);
    Task<IDictionary<string, string>> GetMetadataAsync(string fileName, string containerName);
    
    // Gerenciamento de containers
    Task CreateContainerAsync(string containerName, PublicAccessType accessType = PublicAccessType.None);
    Task DeleteContainerAsync(string containerName);
    Task<bool> ContainerExistsAsync(string containerName);
    
    // Listagens
    IAsyncEnumerable<string> ListFilesAsync(string containerName, string? prefix = null);
    IAsyncEnumerable<string> ListContainersAsync();
    
    //Garante que os containers sejam criados
    Task CreateContainersAsync();
}