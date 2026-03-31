namespace MW.Hosting.AspNetCore.Abstractions;

public interface ICurrentUserAccessor
{
    string? UserId { get; }
    string? UserName { get; }
    bool IsAuthenticated { get; }
}
