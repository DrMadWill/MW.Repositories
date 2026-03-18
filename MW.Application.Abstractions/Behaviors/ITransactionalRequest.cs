namespace MW.Application.Abstractions.Behaviors;

/// <summary>
/// Marker interface for requests that require transactional execution.
/// Pipeline behaviors can detect this marker and wrap the handler execution in a transaction.
/// Infrastructure layer provides the transaction management.
/// </summary>
public interface ITransactionalRequest
{
}
