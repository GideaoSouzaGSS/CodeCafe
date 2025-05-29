namespace CodeCafe.ApiService.Features.Albums.Models;

public class Photo
{
    public Guid Id { get; set; }
    public string Url { get; set; } = string.Empty;
    public Guid AlbumId { get; set; }
}