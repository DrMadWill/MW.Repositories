namespace MW.Core.Rules;

/// <summary>
/// Represents a business rule that can be validated within domain entities.
/// </summary>
public interface IBusinessRule
{
    /// <summary>
    /// Gets the message describing why the rule was broken.
    /// </summary>
    string Message { get; }

    /// <summary>
    /// Determines whether the business rule is broken.
    /// </summary>
    /// <returns><c>true</c> if the rule is broken; otherwise <c>false</c>.</returns>
    bool IsBroken();
}
