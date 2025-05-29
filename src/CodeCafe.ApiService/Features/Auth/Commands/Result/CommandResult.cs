namespace CodeCafe.ApiService.Features.Auth.Commands.Result;

public class CommandResult
{
    public bool Success { get; private set; }
    public string Message { get; private set; }
    public dynamic Data { get; private set; }

    private CommandResult(bool success, string message = null, dynamic data = null)
    {
        Success = success;
        Message = message;
        Data = data;
    }

    public static CommandResult SuccessResult(string message = null, dynamic data = null)
    {
        return new CommandResult(true, message, data);
    }

    public static CommandResult Failure(string message, dynamic data = null)
    {
        return new CommandResult(false, message, data);
    }
}