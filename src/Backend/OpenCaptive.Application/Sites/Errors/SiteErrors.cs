using OpenCaptive.Application.Common;

namespace OpenCaptive.Application.Sites.Errors;

public static class SiteErrors
{
  public static Error NotFound(Guid id) =>
    Error.NotFound("Site.NotFound", $"No site was found with id '{id}'.");

  public static Error SlugAlreadyExists(string slug) =>
    Error.Conflict("Site.SlugAlreadyExists", $"A site with slug '{slug}' already exists in your organization.");
}
