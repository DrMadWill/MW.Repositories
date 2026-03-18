namespace MW.Application.Abstractions.Authorization;

/// <summary>
/// Marker interface for requests that require authorization.
/// Pipeline behaviors can inspect this marker to enforce authorization rules.
/// </summary>
public interface IAuthorizableRequest
{
}
