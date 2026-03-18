namespace MW.Application.Abstractions.Errors;

/// <summary>
/// Base error type used across the application.
/// </summary>
public class Error
{
    public string Code { get; }
    public string Message { get; }

    public Error(string code, string message)
    {
        Code = code;
        Message = message;
    }

    public override string ToString() => $"{Code}: {Message}";
}
