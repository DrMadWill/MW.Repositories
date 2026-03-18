namespace MW.Application.Abstractions.Errors;

/// <summary>
/// Represents a conflict error.
/// </summary>
public class ConflictError : Error
{
    public ConflictError(string message)
        : base("Conflict", message)
    {
    }

    public ConflictError(string code, string message)
        : base(code, message)
    {
    }
}
