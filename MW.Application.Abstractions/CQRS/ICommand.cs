using CSharpFunctionalExtensions;
using MediatR;

namespace MW.Application.Abstractions.CQRS;

/// <summary>
/// Marker interface for a command that does not return a value.
/// Returns <see cref="Result"/> indicating success or failure.
/// </summary>
public interface ICommand : IRequest<Result>
{
}

/// <summary>
/// Marker interface for a command that returns a result of type <typeparamref name="TResult"/>.
/// Returns <see cref="Result{TResult}"/> indicating success with value or failure.
/// </summary>
/// <typeparam name="TResult">The type of the result value.</typeparam>
public interface ICommand<TResult> : IRequest<Result<TResult>>
{
}
