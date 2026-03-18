namespace MW.Application.Abstractions.Errors;

/// <summary>
/// Represents a not found error.
/// </summary>
public class NotFoundError : Error
{
    public NotFoundError(string message)
        : base("NotFound", message)
    {
    }

    public NotFoundError(string code, string message)
        : base(code, message)
    {
    }
}
