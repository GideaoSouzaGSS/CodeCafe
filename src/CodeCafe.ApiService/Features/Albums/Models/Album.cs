namespace CodeCafe.ApiService.Features.Albums.Models;

public class Album
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid UserProfileId { get; set; }
    public List<Photo> Photos { get; set; } = new(); 
}