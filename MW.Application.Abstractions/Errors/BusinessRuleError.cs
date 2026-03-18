namespace MW.Application.Abstractions.Errors;

/// <summary>
/// Represents a business rule violation error.
/// </summary>
public class BusinessRuleError : Error
{
    public BusinessRuleError(string message)
        : base("BusinessRule", message)
    {
    }

    public BusinessRuleError(string code, string message)
        : base(code, message)
    {
    }
}
