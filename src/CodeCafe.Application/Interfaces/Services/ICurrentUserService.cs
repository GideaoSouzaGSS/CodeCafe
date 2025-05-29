namespace CodeCafe.Application.Interfaces.Services;

public interface ICurrentUserService
{
    string Username { get; }
    string Email { get; }
    bool IsAuthenticated { get; }
    IEnumerable<string> Roles { get; }
    Guid ProfileId { get; }
    Guid UserId { get; }
}