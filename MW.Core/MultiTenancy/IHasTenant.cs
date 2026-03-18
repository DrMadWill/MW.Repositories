namespace MW.Core.MultiTenancy;

/// <summary>
/// Represents an entity that belongs to a specific tenant.
/// </summary>
public interface IHasTenant
{
    /// <summary>
    /// Gets or sets the identifier of the tenant that owns this entity.
    /// </summary>
    Guid TenantId { get; set; }
}
