namespace MW.Application.Abstractions.Errors;

/// <summary>
/// Represents an unauthorized error.
/// </summary>
public class UnauthorizedError : Error
{
    public UnauthorizedError(string message)
        : base("Unauthorized", message)
    {
    }

    public UnauthorizedError(string code, string message)
        : base(code, message)
    {
    }
}
