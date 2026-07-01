using OpenCaptive.Domain.Common;

namespace OpenCaptive.Domain.Organizations;

public sealed class Organization : AuditableEntity
{
  public string Name { get; private set; } = null!;
  public string Slug { get; private set; } = null!;
  public bool IsEnabled { get; private set; } = true;

  private Organization() { } // Required by EF Core for materialization.

  public static Organization Create(string name, string slug)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(name);
    Slugs.ThrowIfInvalidSlug(slug);

    return new Organization
    {
      Id = NewId(),
      Name = name,
      Slug = slug,
    };
  }

  public void Rename(string name)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(name);
    Name = name;
  }

  public void ChangeSlug(string slug)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(slug);
    Slugs.ThrowIfInvalidSlug(slug);

    Slug = slug;
  }

  public void Enable()
  {
    IsEnabled = true;
  }

  public void Disable()
  {
    IsEnabled = false;
  }
}