using OpenCaptive.Domain.Common;

namespace OpenCaptive.Domain.Portals;

public sealed class Portal : AuditableEntity
{
  public Guid NetworkId { get; private set; }
  public string Name { get; private set; } = null!;
  public bool IsPublished { get; private set; }
  public Guid? PublishedVersionId { get; private set; }

  private Portal() { } // Required by EF Core for materialization.

  public static Portal Create(Guid networkId, string name)
  {
    ArgumentOutOfRangeException.ThrowIfEqual(networkId, Guid.Empty, nameof(networkId));
    ArgumentException.ThrowIfNullOrWhiteSpace(name);

    return new Portal
    {
      Id = Guid.CreateVersion7(),
      NetworkId = networkId,
      Name = name,
    };
  }

  public void Rename(string name)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(name);
    Name = name;
  }

  public void Publish(Guid versionId)
  {
    ArgumentOutOfRangeException.ThrowIfEqual(versionId, Guid.Empty, nameof(versionId));

    IsPublished = true;
    PublishedVersionId = versionId;
  }

  public void Unpublish()
  {
    IsPublished = false;
    PublishedVersionId = null;
  }
}