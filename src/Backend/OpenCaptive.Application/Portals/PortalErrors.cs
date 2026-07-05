using OpenCaptive.Application.Common;

namespace OpenCaptive.Application.Portals;

public static class PortalErrors
{
  public static Error NotFound(Guid networkId) =>
    Error.NotFound("Portal.NotFound", $"Network '{networkId}' does not have a portal yet.");

  public static Error AlreadyExists(Guid networkId) =>
    Error.Conflict("Portal.AlreadyExists", $"Network '{networkId}' already has a portal.");

  public static Error VersionNotFound(Guid versionId) =>
    Error.NotFound("Portal.VersionNotFound", $"No portal version was found with id '{versionId}'.");
}
