namespace MW.Messaging.Validation;

/// <summary>
/// Validates an <see cref="Contracts.IIntegrationEvent"/> against required event contract rules.
/// <para>
/// This abstraction defines the validation contract. Concrete validation logic belongs
/// to the implementation or policy layer, not to this package.
/// </para>
/// <para>
/// Expected validations include (but are not limited to):
/// <list type="bullet">
///   <item><description><c>EventName</c> must not be empty</description></item>
///   <item><description><c>EventVersion</c> must be a valid version string</description></item>
///   <item><description><c>SourceService</c> must be present</description></item>
///   <item><description>Correlation metadata rules, if applicable</description></item>
/// </list>
/// </para>
/// </summary>
public interface IIntegrationEventValidator
{
    /// <summary>
    /// Validates the given integration event against required contract rules.
    /// Throws an exception if the event does not satisfy the rules.
    /// </summary>
    /// <param name="integrationEvent">The integration event to validate.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="integrationEvent"/> is <c>null</c>.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the event does not satisfy required contract rules.</exception>
    void Validate(Contracts.IIntegrationEvent integrationEvent);
}
