using OpenCaptive.Integrations.Abstractions.Models;

namespace OpenCaptive.Integrations.Abstractions.GuestAccess;

public interface IGuestAccessIntegration
{
  Task<bool> AuthorizeClientAsync(string macAddress, CancellationToken cancellationToken = default);
  Task<bool> DisconnectClientAsync(string macAddress, CancellationToken cancellationToken = default);
  Task<GuestClient?> GetClientAsync(string macAddress, CancellationToken cancellationToken = default);
}