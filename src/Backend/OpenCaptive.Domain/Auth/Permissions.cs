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
}
