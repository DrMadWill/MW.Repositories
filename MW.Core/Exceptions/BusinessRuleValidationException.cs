using MW.Core.Rules;

namespace MW.Core.Exceptions;

/// <summary>
/// Exception thrown when a business rule is violated.
/// </summary>
public sealed class BusinessRuleValidationException : Exception
{
    /// <summary>
    /// Gets the broken business rule.
    /// </summary>
    public IBusinessRule BrokenRule { get; }

    public BusinessRuleValidationException(IBusinessRule brokenRule)
        : base(brokenRule.Message)
    {
        BrokenRule = brokenRule;
    }
}
