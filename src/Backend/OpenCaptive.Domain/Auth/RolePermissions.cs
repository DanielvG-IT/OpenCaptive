using OpenCaptive.Domain.Organizations;

namespace OpenCaptive.Domain.Auth;

public static class RolePermissions
{
  private static readonly Dictionary<OrganizationRole, IReadOnlySet<string>> Map = new()
  {
    [OrganizationRole.Owner] = new HashSet<string>
    {
      Permissions.Organizations.Read,
      Permissions.Organizations.Update,
      Permissions.Organizations.Delete,

      Permissions.Members.Read,
      Permissions.Members.Add,
      Permissions.Members.Remove,

      Permissions.Sites.Create,
      Permissions.Sites.ReadAll,
      Permissions.Sites.ReadOne,
      Permissions.Sites.Update,
      Permissions.Sites.Delete
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