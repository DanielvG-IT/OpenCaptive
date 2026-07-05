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
      Permissions.Sites.Delete,

      // Owner-only for now, matching the Sites precedent above — revisit once you decide
      // whether Administrator/Editor should manage networks/integrations/portals/invitations.
      Permissions.Networks.Create,
      Permissions.Networks.ReadAll,
      Permissions.Networks.ReadOne,
      Permissions.Networks.Update,
      Permissions.Networks.Delete,

      Permissions.Integrations.Create,
      Permissions.Integrations.ReadAll,
      Permissions.Integrations.ReadOne,
      Permissions.Integrations.Update,
      Permissions.Integrations.Delete,
      Permissions.Integrations.Connect,
      Permissions.Integrations.Disconnect,
      Permissions.Integrations.Sync,

      Permissions.Portals.Read,
      Permissions.Portals.Create,
      Permissions.Portals.Update,
      Permissions.Portals.Publish,

      Permissions.Invitations.Read,
      Permissions.Invitations.Create,
      Permissions.Invitations.Revoke,
      Permissions.Invitations.Resend,
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