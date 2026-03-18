namespace MW.Application.Abstractions.CQRS;

/// <summary>
/// Marker interface for a query that returns a result of type <typeparamref name="TResult"/>.
/// </summary>
/// <typeparam name="TResult">The type of the result.</typeparam>
public interface IQuery<TResult>
{
}
