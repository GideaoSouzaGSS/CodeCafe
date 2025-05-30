using Azure.Storage.Blobs.Models;
using CodeCafe.Application.Interfaces.Services;

public class FakeBlobService : IBlobService, IProfileImageBlobService, IPrivateImageBlobService
{
    // Implemente todos os métodos das interfaces retornando Task.CompletedTask ou valores default
    public Task<bool> ContainerExistsAsync(string containerName)
    {
        throw new NotImplementedException();
    }

    public Task CreateContainerAsync(string containerName, PublicAccessType accessType = PublicAccessType.None)
    {
        throw new NotImplementedException();
    }

    public Task CreateContainersAsync()
    {
        throw new NotImplementedException();
    }

    public Task DeleteContainerAsync(string containerName)
    {
        throw new NotImplementedException();
    }

    public Task DeleteFileAsync(string fileName, string containerName)
    {
        throw new NotImplementedException();
    }

    public Task DeleteFileAsync(string fileName)
    {
        throw new NotImplementedException();
    }

    public Task<Stream> DownloadFileAsync(string fileName, string containerName)
    {
        throw new NotImplementedException();
    }

    public Task<(Stream Content, string ContentType)> DownloadFileAsync(string fileName)
    {
        throw new NotImplementedException();
    }

    public Task<Stream> DownloadFilePrivateAsync(string fileName)
    {
        throw new NotImplementedException();
    }

    public Task<bool> FileExistsAsync(string fileName, string containerName)
    {
        throw new NotImplementedException();
    }

    public Task<string> GenerateSasTokenAsync(string fileName, string containerName, TimeSpan expiryTime)
    {
        throw new NotImplementedException();
    }

    public Task<IDictionary<string, string>> GetMetadataAsync(string fileName, string containerName)
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<string> ListContainersAsync()
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<string> ListFilesAsync(string containerName, string? prefix = null)
    {
        throw new NotImplementedException();
    }

    public Task SetMetadataAsync(string fileName, string containerName, IDictionary<string, string> metadata)
    {
        throw new NotImplementedException();
    }

    public Task<string> UploadFileAsync(Stream fileStream, string fileName, string containerName, string? contentType = null)
    {
        throw new NotImplementedException();
    }

    public Task<string> UploadFileAsync(Stream fileStream, string fileName, string? contentType = null)
    {
        throw new NotImplementedException();
    }
}