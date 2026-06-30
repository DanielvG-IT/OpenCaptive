using OpenCaptive.Domain.Common;

namespace OpenCaptive.Domain.Portals;

public sealed class PortalVersion : AuditableEntity
{
  public Guid PortalId { get; private set; }
  public int Version { get; private set; }
  public bool IsPublished { get; private set; }
  public string Content { get; private set; } = "{}";

  private PortalVersion() { } // Required by EF Core for materialization.

  public static PortalVersion Create(Guid portalId, int version, string content)
  {
    ArgumentOutOfRangeException.ThrowIfEqual(portalId, Guid.Empty);
    ArgumentOutOfRangeException.ThrowIfLessThan(version, 1);
    ArgumentException.ThrowIfNullOrWhiteSpace(content);

    return new PortalVersion
    {
      Id = Guid.CreateVersion7(),
      PortalId = portalId,
      Version = version,
      IsPublished = false,
      Content = content
    };
  }

  public void MarkPublished()
  {
    IsPublished = true;
  }
}