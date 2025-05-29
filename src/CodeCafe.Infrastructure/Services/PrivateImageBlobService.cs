using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;
using Microsoft.Extensions.Options;
using CodeCafe.Application.Interfaces.Services;
using CodeCafe.Infrastructure.Configuration;
using Microsoft.Extensions.Logging;
namespace CodeCafe.Infrastructure.Services;

public class PrivateImageBlobService : IPrivateImageBlobService
{
    private readonly BlobStorageOptions _options;
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _containerName;
    private readonly ILogger<PrivateImageBlobService> _logger;

    public PrivateImageBlobService(
        BlobServiceClient blobServiceClient,
        IOptions<BlobStorageOptions> options,
        ILogger<PrivateImageBlobService> logger)
    {
        _blobServiceClient = blobServiceClient ?? throw new ArgumentNullException(nameof(blobServiceClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _containerName = _options.PrivateImagesContainer ?? 
            throw new ArgumentNullException("BlobStorage:PrivateImagesContainer configuration is missing.");
    }

    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string? contentType = null)
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.None);

            var blobClient = containerClient.GetBlobClient(fileName);

            var options = new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders
                {
                    ContentType = contentType ?? GetContentType(fileName)
                }
            };

            _logger.LogInformation("Uploading file {FileName} to container {ContainerName}", fileName, _containerName);
            await blobClient.UploadAsync(fileStream, options);

            return fileName;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file {FileName}", fileName);
            throw;
        }
    }

    public async Task<(Stream Content, string ContentType)> DownloadFileAsync(string fileName)
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(fileName);

            if (!await blobClient.ExistsAsync())
            {
                _logger.LogWarning("File {FileName} not found in container {ContainerName}", fileName, _containerName);
                throw new FileNotFoundException($"File {fileName} not found in container {_containerName}");
            }

            var properties = await blobClient.GetPropertiesAsync();
            var stream = new MemoryStream();
            await blobClient.DownloadToAsync(stream);
            stream.Position = 0;

            _logger.LogInformation("Downloaded file {FileName} from container {ContainerName}", fileName, _containerName);
            return (stream, properties.Value.ContentType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading file {FileName}", fileName);
            throw;
        }
    }

    public async Task DeleteFileAsync(string fileName)
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(fileName);

            _logger.LogInformation("Deleting file {FileName} from container {ContainerName}", fileName, _containerName);
            await blobClient.DeleteIfExistsAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file {FileName}", fileName);
            throw;
        }
    }

    private static string GetContentType(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return extension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            _ => "application/octet-stream"
        };
    }
}