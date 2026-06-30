using OpenCaptive.Domain.Common;

namespace OpenCaptive.Domain.Organizations;

public sealed class Organization : AuditableEntity
{
  public string Name { get; private set; } = default!;

  public string Slug { get; private set; } = default!;

  // Required by EF Core for materialization.
  private Organization()
  {
  }

  public static Organization Create(string name, string slug)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(name);
    ArgumentException.ThrowIfNullOrWhiteSpace(slug);
    Slugs.ThrowIfInvalidSlug(slug);

    return new Organization
    {
      Id = Guid.CreateVersion7(),
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
}