using CSharpFunctionalExtensions;

namespace MW.Application.Abstractions.CQRS;

/// <summary>
/// Handler for a query that returns a result of type <typeparamref name="TResult"/>.
/// </summary>
/// <typeparam name="TQuery">The type of the query.</typeparam>
/// <typeparam name="TResult">The type of the result.</typeparam>
public interface IQueryHandler<in TQuery, TResult>
    where TQuery : IQuery<TResult>
{
    Task<Result<TResult>> Handle(TQuery query, CancellationToken cancellationToken = default);
}
