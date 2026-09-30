namespace ApiTemplate.Domain.Common;

/// <summary>
/// Base entity with identity and audit fields shared by all domain entities.
/// </summary>
public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public DateTime CreatedAtUtc { get; set; }

    public DateTime? UpdatedAtUtc { get; set; }
}
