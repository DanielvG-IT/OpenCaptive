namespace OpenCaptive.Domain.Auth;

public static class Permissions
{
  public static class Organizations
  {
    public const string Read = "organizations.read";
    public const string Update = "organizations.update";
    public const string Delete = "organizations.delete";

    public static class Members
    {
      public const string Read = "organizations.members.read";
      public const string Add = "organizations.members.add";
      public const string Remove = "organizations.members.remove";
    }
  }

  public static class Sites
  {
    public const string Read = "sites.read";
    public const string Create = "sites.create";
    public const string Update = "sites.update";
    public const string Delete = "sites.delete";
  }
}
