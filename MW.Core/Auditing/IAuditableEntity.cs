namespace MW.Core.Auditing;

/// <summary>
/// Represents an entity that tracks both creation and update times.
/// Combines <see cref="ICreationAudited"/> and <see cref="IUpdateAudited"/>.
/// </summary>
public interface IAuditableEntity : ICreationAudited, IUpdateAudited
{
}
