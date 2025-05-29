namespace CodeCafe.ApiService.Features.Profile.Queries;

public class UpdateProfileRequestDto
{
    public string Username { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public string? Bio { get; set; }
    public bool AcceptFollow { get; set; }
    public bool AcceptDirectMessage { get; set; }
}