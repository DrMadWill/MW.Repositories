using MW.Core.Auditing;
using MW.Core.Entities;

namespace MW.Core.Models.Abstractions
{
    /// <summary>
    /// Represents the base interface for entities combining identity, soft delete, and auditing.
    /// </summary>
    /// <typeparam name="T">The type of the Id property.</typeparam>
    public interface IBaseEntity<T> : IOriginEntity<T>, ISoftDelete, IAuditableEntity
    {
    }
}