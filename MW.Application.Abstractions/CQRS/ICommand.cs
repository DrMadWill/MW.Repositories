using CSharpFunctionalExtensions;

namespace MW.Application.Abstractions.CQRS;

/// <summary>
/// Marker interface for a command that does not return a value.
/// </summary>
public interface ICommand
{
}

/// <summary>
/// Marker interface for a command that returns a result of type <typeparamref name="TResult"/>.
/// </summary>
/// <typeparam name="TResult">The type of the result.</typeparam>
public interface ICommand<TResult>
{
}
