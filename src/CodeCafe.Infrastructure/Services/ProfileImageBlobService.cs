using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;
using Microsoft.Extensions.Options;
using CodeCafe.Application.Interfaces.Services;
using CodeCafe.Infrastructure.Configuration;
using Microsoft.Extensions.Logging;

namespace CodeCafe.Infrastructure.Services;

public class ProfileImageBlobService : IProfileImageBlobService
{
    private readonly BlobStorageOptions _options;
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _containerName;
    private readonly ILogger<ProfileImageBlobService> _logger;

    public ProfileImageBlobService(
        BlobServiceClient blobServiceClient,
        IOptions<BlobStorageOptions> options,
        ILogger<ProfileImageBlobService> logger)
    {
        _blobServiceClient = blobServiceClient ?? throw new ArgumentNullException(nameof(blobServiceClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _containerName = _options.ProfileImagesContainer ?? 
            throw new ArgumentNullException("BlobStorage:ProfileImagesContainer configuration is missing.");
    }

    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string? contentType = null)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

        var blobClient = containerClient.GetBlobClient(fileName);
        await blobClient.UploadAsync(fileStream, new BlobHttpHeaders { ContentType = contentType });

        return blobClient.Uri.ToString();
    }
}