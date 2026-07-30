using OpenCaptive.Application.Common;

namespace OpenCaptive.Application.Integrations;

public static class SiteIntegrationErrors
{
  public static Error NotFound(Guid id) =>
    Error.NotFound("SiteIntegration.NotFound", $"No integration was found with id '{id}'.");

  public static Error CannotDeleteWithActiveNetworks(Guid id) =>
    Error.Conflict("SiteIntegration.CannotDelete", $"Integration '{id}' cannot be deleted because it still has networks synced from it.");

  public static Error ConnectionFailed(Guid id, string reason) =>
    Error.Failure("SiteIntegration.ConnectionFailed", $"Could not connect integration '{id}': {reason}");
}
