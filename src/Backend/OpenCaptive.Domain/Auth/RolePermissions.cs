using OpenCaptive.Domain.Organizations;

namespace OpenCaptive.Domain.Auth;

public static class RolePermissions
{
  private static readonly IReadOnlyDictionary<OrganizationRole, IReadOnlySet<string>> Map = new Dictionary<OrganizationRole, IReadOnlySet<string>>
  {
    [OrganizationRole.Owner] = new HashSet<string>
    {
      Permissions.Organizations.Read,
      Permissions.Organizations.Update,
      Permissions.Organizations.Delete,
    },
    [OrganizationRole.Administrator] = new HashSet<string>
    {
      Permissions.Organizations.Read,
      Permissions.Organizations.Update,
    },
    [OrganizationRole.Editor] = new HashSet<string>
    {
      Permissions.Organizations.Read,
    },
  };

  public static bool RoleHasPermission(OrganizationRole role, string permission)
  {
    return Map.TryGetValue(role, out var permissions) && permissions.Contains(permission);
  }
}