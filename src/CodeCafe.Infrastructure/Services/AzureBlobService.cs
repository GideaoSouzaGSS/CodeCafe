using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;
using Microsoft.Extensions.Options;
using CodeCafe.Application.Interfaces.Services;
using CodeCafe.Infrastructure.Configuration;
using Microsoft.Extensions.Logging; 

namespace CodeCafe.Infrastructure.Services;

public class AzureBlobService : IBlobService
{
    private readonly BlobStorageOptions _options;
    private readonly BlobServiceClient _blobServiceClient;
    private readonly ILogger<AzureBlobService> _logger;

    private string ProfileImagesContainerName => _options.ProfileImagesContainer;
    private string PrivateImagesContainerName => _options.PrivateImagesContainer;
    private string PublicImagesContainerName => _options.PublicImagesContainer;

    public AzureBlobService(
        BlobServiceClient blobServiceClient,
        ILogger<AzureBlobService> logger,
        IOptions<BlobStorageOptions> options)
    {
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _blobServiceClient = blobServiceClient ?? throw new ArgumentNullException(nameof(blobServiceClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string containerName, string? contentType = null)
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

            await containerClient.SetAccessPolicyAsync(PublicAccessType.Blob);

            var blobClient = containerClient.GetBlobClient(fileName);

            var options = new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders
                {
                    ContentType = contentType ?? GetContentType(fileName)
                }
            };

            await blobClient.UploadAsync(fileStream, options);
            return blobClient.Uri.ToString();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao fazer upload do arquivo {FileName} no container {ContainerName}", fileName, containerName);
            throw;
        }
    }

    public async Task<Stream> DownloadFileAsync(string fileName, string containerName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(fileName);

        var stream = new MemoryStream();
        await blobClient.DownloadToAsync(stream);
        stream.Position = 0;

        return stream;
    }

    public async Task<Stream> DownloadFilePrivateAsync(string fileName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient("private-images");
        var blobClient = containerClient.GetBlobClient(fileName);

        // Verifica se o blob existe
        if (!await blobClient.ExistsAsync())
        {
            throw new FileNotFoundException($"O blob '{fileName}' não existe no contêiner private-images.");
        }

        var stream = new MemoryStream();
        await blobClient.DownloadToAsync(stream);
        stream.Position = 0;

        return stream;
    }
    public async Task DeleteFileAsync(string fileName, string containerName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(fileName);

        await blobClient.DeleteIfExistsAsync();
    }

    public async Task<bool> FileExistsAsync(string fileName, string containerName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(fileName);

        return await blobClient.ExistsAsync();
    }

    public async Task<string> GenerateSasTokenAsync(string fileName, string containerName, TimeSpan expiryTime)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(fileName);

        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = containerName,
            BlobName = fileName,
            Resource = "b",
            ExpiresOn = DateTimeOffset.UtcNow.Add(expiryTime)
        };

        sasBuilder.SetPermissions(BlobSasPermissions.Read);

        var sasToken = blobClient.GenerateSasUri(sasBuilder).Query;
        return sasToken;
    }

    public async Task<string> GenerateSasUrlAsync(string fileName, string containerName, TimeSpan expiryTime)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(fileName);

        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = containerName,
            BlobName = fileName,
            Resource = "b",
            ExpiresOn = DateTimeOffset.UtcNow.Add(expiryTime)
        };

        sasBuilder.SetPermissions(BlobSasPermissions.Read);

        var sasUri = blobClient.GenerateSasUri(sasBuilder);
        return sasUri.ToString();
    }

    public async Task SetMetadataAsync(string fileName, string containerName, IDictionary<string, string> metadata)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(fileName);

        await blobClient.SetMetadataAsync(metadata);
    }

    public async Task<IDictionary<string, string>> GetMetadataAsync(string fileName, string containerName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(fileName);

        var properties = await blobClient.GetPropertiesAsync();
        return properties.Value.Metadata;
    }

    public async Task CreateContainerAsync(string containerName, PublicAccessType accessType = PublicAccessType.None)
    {
        await _blobServiceClient.CreateBlobContainerAsync(containerName, accessType);
    }

    public async Task DeleteContainerAsync(string containerName)
    {
        await _blobServiceClient.DeleteBlobContainerAsync(containerName);
    }

    public async Task<bool> ContainerExistsAsync(string containerName)
    {
        return await _blobServiceClient.GetBlobContainerClient(containerName).ExistsAsync();
    }

    public async IAsyncEnumerable<string> ListFilesAsync(string containerName, string? prefix = null)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

        await foreach (var blobItem in containerClient.GetBlobsAsync(prefix: prefix))
        {
            yield return blobItem.Name;
        }
    }

    public async IAsyncEnumerable<string> ListContainersAsync()
    {
        await foreach (var containerItem in _blobServiceClient.GetBlobContainersAsync())
        {
            yield return containerItem.Name;
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
            ".pdf" => "application/pdf",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            _ => "application/octet-stream"
        };
    }

    public async Task CreateContainersAsync()
    {
        ValidateConfiguration();

        var containerClient = _blobServiceClient.GetBlobContainerClient(ProfileImagesContainerName);
        await containerClient.CreateIfNotExistsAsync(PublicAccessType.None);

        var containerClient2 = _blobServiceClient.GetBlobContainerClient(PrivateImagesContainerName);
        await containerClient2.CreateIfNotExistsAsync(PublicAccessType.None); // Changed from Blob to None

        var containerClient3 = _blobServiceClient.GetBlobContainerClient(PublicImagesContainerName);
        await containerClient3.CreateIfNotExistsAsync(PublicAccessType.None);

        // Set access policy explicitly to ensure privacy
        await containerClient.SetAccessPolicyAsync(PublicAccessType.None);
        await containerClient2.SetAccessPolicyAsync(PublicAccessType.None);
        await containerClient3.SetAccessPolicyAsync(PublicAccessType.None);
    }

    private void ValidateConfiguration()
    {
        if (string.IsNullOrEmpty(ProfileImagesContainerName) ||
            string.IsNullOrEmpty(PrivateImagesContainerName) ||
            string.IsNullOrEmpty(PublicImagesContainerName))
        {
            throw new InvalidOperationException("Os nomes dos contêineres não estão configurados corretamente no appsettings.json.");
        }
    }
}
