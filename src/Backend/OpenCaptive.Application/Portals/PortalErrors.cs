using OpenCaptive.Application.Common;

namespace OpenCaptive.Application.Portals;

public static class PortalErrors
{
  public static Error NotFound(Guid networkId) =>
    Error.NotFound("Portal.NotFound", $"No portal was found for network '{networkId}'.");

  public static Error VersionNotFound(Guid versionId) =>
    Error.NotFound("Portal.VersionNotFound", $"No portal version was found with id '{versionId}'.");

  public static Error CannotDeletePublishedVersion(Guid versionId) =>
    Error.Conflict("Portal.CannotDeletePublishedVersion", $"Version '{versionId}' has been published and can never be deleted — only draft (never-published) versions may be removed.");
}
