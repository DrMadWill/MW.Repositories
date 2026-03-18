using CSharpFunctionalExtensions;

namespace MW.Application.Abstractions.Validation;

/// <summary>
/// Defines a validator for application requests.
/// Validators run before request execution in the pipeline.
/// Compatible with FluentValidation or custom validation implementations.
/// </summary>
/// <typeparam name="TRequest">The type of the request to validate.</typeparam>
public interface IRequestValidator<in TRequest>
{
    /// <summary>
    /// Validates the specified request.
    /// Returns <see cref="Result.Success()"/> if validation passes,
    /// or <see cref="Result.Failure(string)"/> with a validation error message.
    /// </summary>
    /// <param name="request">The request to validate.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A <see cref="Result"/> indicating validation success or failure.</returns>
    Task<Result> ValidateAsync(TRequest request, CancellationToken cancellationToken = default);
}
