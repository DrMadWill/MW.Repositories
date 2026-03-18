namespace MW.Application.Abstractions.Errors;

/// <summary>
/// Represents a forbidden error.
/// </summary>
public class ForbiddenError : Error
{
    public ForbiddenError(string message)
        : base("Forbidden", message)
    {
    }

    public ForbiddenError(string code, string message)
        : base(code, message)
    {
    }
}
