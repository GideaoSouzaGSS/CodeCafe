using System;

namespace CodeCafe.Infrastructure.Configuration;

public class BlobStorageOptions
{
    public const string ConfigurationSection = "BlobStorage";
    
    public string ConnectionString { get; set; } = string.Empty;
    public string ProfileImagesContainer { get; set; } = string.Empty;
    public string PrivateImagesContainer { get; set; } = string.Empty;
    public string PublicImagesContainer { get; set; } = string.Empty;
}