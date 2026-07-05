namespace OpenCaptive.Domain.Auth;

public static class Permissions
{
  public static class Organizations
  {
    public const string Read = "organizations.read";
    public const string Update = "organizations.update";
    public const string Delete = "organizations.delete";
  }

  public static class Members
  {
    public const string Read = "members.read";
    public const string Add = "members.add";
    public const string Remove = "members.remove";
  }

  public static class Sites
  {
    public const string ReadAll = "sites.readAll";
    public const string ReadOne = "sites.readOne";
    public const string Create = "sites.create";
    public const string Update = "sites.update";
    public const string Delete = "sites.delete";
  }

  public static class Networks
  {
    public const string ReadAll = "networks.readAll";
    public const string ReadOne = "networks.readOne";
    public const string Create = "networks.create";
    public const string Update = "networks.update";
    public const string Delete = "networks.delete";
  }

  public static class Integrations
  {
    public const string ReadAll = "integrations.readAll";
    public const string ReadOne = "integrations.readOne";
    public const string Create = "integrations.create";
    public const string Update = "integrations.update";
    public const string Delete = "integrations.delete";
    public const string Connect = "integrations.connect";
    public const string Disconnect = "integrations.disconnect";
    public const string Sync = "integrations.sync";
  }

  public static class Portals
  {
    public const string Read = "portals.read";
    public const string Create = "portals.create";
    public const string Update = "portals.update";
    public const string Publish = "portals.publish";
  }

  public static class Invitations
  {
    public const string Read = "invitations.read";
    public const string Create = "invitations.create";
    public const string Revoke = "invitations.revoke";
    public const string Resend = "invitations.resend";
  }
}
