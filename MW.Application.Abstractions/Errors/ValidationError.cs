namespace MW.Application.Abstractions.Errors;

/// <summary>
/// Represents a validation error.
/// </summary>
public class ValidationError : Error
{
    public ValidationError(string message)
        : base("Validation", message)
    {
    }

    public ValidationError(string code, string message)
        : base(code, message)
    {
    }
}
