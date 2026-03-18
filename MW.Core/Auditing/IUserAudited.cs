namespace MW.Core.Auditing;

/// <summary>
/// Represents an entity that tracks user information for creation, update, and deletion.
/// Combines <see cref="ICreatedAudit"/>, <see cref="IUpdatedAudit"/>, and <see cref="IDeletedAudit"/>.
/// </summary>
public interface IUserAudited : ICreatedAudit, IUpdatedAudit, IDeletedAudit
{
}
