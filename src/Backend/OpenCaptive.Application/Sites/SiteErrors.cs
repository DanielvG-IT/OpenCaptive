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

  public static Error CannotDeleteBecauseHasNetworks(Guid id) =>
      Error.Conflict("Site.CannotDeleteNetworks", $"Site '{id}' cannot be deleted. Remove its associated networks before deleting.");

  public static Error CannotDeleteBecauseHasIntegrations(Guid id) =>
      Error.Conflict("Site.CannotDeleteIntegrations", $"Site '{id}' cannot be deleted. Remove its active integrations before deleting.");
}
