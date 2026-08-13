using OpenCaptive.Integrations.Abstractions.Models;

namespace OpenCaptive.Integrations.Abstractions.Networks;

public interface INetworkIntegration
{
  Task<IReadOnlyList<Network>> GetNetworksAsync(CancellationToken cancellationToken = default);
  Task<Network?> GetNetworkAsync(string providerNetworkId, CancellationToken cancellationToken = default);
}