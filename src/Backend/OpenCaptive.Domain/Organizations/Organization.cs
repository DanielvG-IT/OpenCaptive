using OpenCaptive.Domain.Common;

namespace OpenCaptive.Domain.Organizations;

public sealed class Organization : AuditableEntity
{
  public string Name { get; set; } = default!;
  public string Slug { get; set; } = default!;
}