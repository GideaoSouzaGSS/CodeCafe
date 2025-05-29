namespace CodeCafe.Domain.Entities;

public class Album
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<Photo> Photos { get; set; } = new();

    // Relacionamento com UserProfile
    public Guid UserProfileId { get; set; }
}
