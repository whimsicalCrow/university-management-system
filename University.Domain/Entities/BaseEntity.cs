namespace University.Domain.Entities;

/// <summary>
/// Base class for all domain entities, providing common properties.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Gets or sets the unique identifier for this entity.
    /// </summary>
    public int Id { get; set; }
}
