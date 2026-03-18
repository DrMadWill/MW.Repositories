using CSharpFunctionalExtensions;

namespace MW.Application.Abstractions.CQRS;

/// <summary>
/// Handler for a command that does not return a value.
/// </summary>
/// <typeparam name="TCommand">The type of the command.</typeparam>
public interface ICommandHandler<in TCommand>
    where TCommand : ICommand
{
    Task<Result> Handle(TCommand command, CancellationToken cancellationToken = default);
}

/// <summary>
/// Handler for a command that returns a result of type <typeparamref name="TResult"/>.
/// </summary>
/// <typeparam name="TCommand">The type of the command.</typeparam>
/// <typeparam name="TResult">The type of the result.</typeparam>
public interface ICommandHandler<in TCommand, TResult>
    where TCommand : ICommand<TResult>
{
    Task<Result<TResult>> Handle(TCommand command, CancellationToken cancellationToken = default);
}
