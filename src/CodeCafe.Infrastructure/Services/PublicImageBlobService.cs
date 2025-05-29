using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;
using Microsoft.Extensions.Options;
using CodeCafe.Application.Interfaces.Services;
using CodeCafe.Infrastructure.Configuration;
using Microsoft.Extensions.Logging;

namespace CodeCafe.Infrastructure.Services;

public class PublicImageBlobService : IPublicImageBlobService
{
    private readonly BlobStorageOptions _options;
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _containerName;
    private readonly ILogger<PublicImageBlobService> _logger;

    public PublicImageBlobService(
        BlobServiceClient blobServiceClient,
        IOptions<BlobStorageOptions> options,
        ILogger<PublicImageBlobService> logger)
    {
        _blobServiceClient = blobServiceClient ?? throw new ArgumentNullException(nameof(blobServiceClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _containerName = _options.PublicImagesContainer ?? 
            throw new ArgumentNullException("BlobStorage:PublicImagesContainer configuration is missing.");
    }

    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string? contentType = null)
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

            var blobClient = containerClient.GetBlobClient(fileName);

            var options = new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders
                {
                    ContentType = contentType
                }
            };

            _logger.LogInformation("Uploading file {FileName} to container {ContainerName}", fileName, _containerName);
            await blobClient.UploadAsync(fileStream, options);

            return blobClient.Uri.ToString();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file {FileName} to container {ContainerName}", fileName, _containerName);
            throw;
        }
    }
}