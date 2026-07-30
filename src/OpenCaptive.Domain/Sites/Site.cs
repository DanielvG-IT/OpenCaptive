using OpenCaptive.Domain.Common;

namespace OpenCaptive.Domain.Sites;

public sealed class Site : AuditableEntity
{
  public Guid OrganizationId { get; private set; }
  public string Name { get; private set; } = null!;
  public string Slug { get; private set; } = null!;
  public string? Description { get; private set; }
  public string TimeZone { get; private set; } = "UTC";
  public bool IsEnabled { get; private set; } = true;

  private Site() { } // Required by EF Core for materialization.

  public static Site Create(Guid organizationId, string name, string slug, string timeZone, string? description)
  {
    ArgumentOutOfRangeException.ThrowIfEqual(organizationId, Guid.Empty);
    ArgumentException.ThrowIfNullOrWhiteSpace(name);
    Slugs.ThrowIfInvalidSlug(slug);

    return new Site
    {
      Id = NewId(),
      Name = name,
      Slug = slug,
      TimeZone = timeZone,
      Description = description,
      IsEnabled = true,
      OrganizationId = organizationId
    };
  }

  public void UpdateName(string name)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(name);
    Name = name;
  }

  public void UpdateSlug(string slug)
  {
    Slugs.ThrowIfInvalidSlug(slug);
    Slug = slug;
  }

  public void UpdateTimeZone(string timeZone)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(timeZone);
    TimeZone = timeZone;
  }

  public void UpdateDescription(string? description)
  {
    Description = description;
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