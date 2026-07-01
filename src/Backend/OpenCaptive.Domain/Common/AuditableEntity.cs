namespace OpenCaptive.Domain.Common;

public abstract class AuditableEntity
{
  public Guid Id { get; protected set; }
  public DateTimeOffset CreatedAt { get; protected set; }
  public DateTimeOffset UpdatedAt { get; protected set; }

  protected static Guid NewId() => Guid.CreateVersion7();
}