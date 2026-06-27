using OpenCaptive.Domain.Common;

namespace OpenCaptive.Domain.Organizations;

public sealed class OrganizationMembership : AuditableEntity
{
  public Guid UserId { get; private set; }
  public Guid OrganizationId { get; private set; }
  public OrganizationRole Role { get; private set; }

  // Required by EF Core for materialization.
  private OrganizationMembership()
  {
  }

  public static OrganizationMembership Create(Guid userId, Guid orgId, OrganizationRole role)
  {
    if (userId == Guid.Empty)
      throw new ArgumentException("User ID must not be empty.", nameof(userId));

    if (orgId == Guid.Empty)
      throw new ArgumentException("Organization ID must not be empty.", nameof(orgId));

    if (!Enum.IsDefined(role))
      throw new ArgumentException("Role must be a valid OrganizationRole value.", nameof(role));

    return new OrganizationMembership
    {
      Id = Guid.CreateVersion7(),
      UserId = userId,
      OrganizationId = orgId,
      Role = role
    };
  }
}