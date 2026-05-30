namespace OpenCaptive.Domain.Common;

public abstract class Entity
{
  public Guid Id { get; protected set; }
}

public abstract class AuditableEntity : Entity
{
  public DateTime CreatedAt { get; protected set; }
  public DateTime UpdatedAt { get; protected set; }
}