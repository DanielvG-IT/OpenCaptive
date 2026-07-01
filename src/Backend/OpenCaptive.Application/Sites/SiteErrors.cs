using OpenCaptive.Application.Common;

namespace OpenCaptive.Application.Sites;

public static class SiteErrors
{
  public static Error NotFound(Guid id) =>
    Error.NotFound("Site.NotFound", $"No site was found with id '{id}'.");

  public static Error SlugAlreadyExists(string slug) =>
    Error.Conflict("Site.SlugAlreadyExists", $"A site with slug '{slug}' already exists in your organization.");

  public static Error CannotDeleteWithActiveDependencies(Guid id) =>
    Error.Conflict("Site.CannotDelete", $"Site '{id}' cannot be deleted because it still has active integrations, networks, or portals.");
}
