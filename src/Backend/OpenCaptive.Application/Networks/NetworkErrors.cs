using OpenCaptive.Application.Common;

namespace OpenCaptive.Application.Networks;

public static class NetworkErrors
{
  public static Error NotFound(Guid id) =>
    Error.NotFound("Network.NotFound", $"No network was found with id '{id}'.");

  public static Error CannotDeleteWithActivePortal(Guid id) =>
    Error.Conflict("Network.CannotDelete", $"Network '{id}' cannot be deleted because it still has a portal. Remove the portal first.");
}
