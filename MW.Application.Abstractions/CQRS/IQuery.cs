using CSharpFunctionalExtensions;
using MediatR;

namespace MW.Application.Abstractions.CQRS;

/// <summary>
/// Marker interface for a query that returns a result of type <typeparamref name="TResult"/>.
/// Returns <see cref="Result{TResult}"/> indicating success with value or failure.
/// </summary>
/// <typeparam name="TResult">The type of the result value.</typeparam>
public interface IQuery<TResult> : IRequest<Result<TResult>>
{
}
