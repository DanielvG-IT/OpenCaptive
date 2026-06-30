using OpenCaptive.Domain.Common;

namespace OpenCaptive.Domain.Networks;

public sealed class Network : AuditableEntity
{
  public Guid SiteId { get; private set; }
  public Guid SiteIntegrationId { get; private set; }
  public string Name { get; private set; } = null!;
  public string ProviderNetworkId { get; private set; } = null!;
  public bool IsGuestNetwork { get; private set; }
  public bool IsEnabled { get; private set; }

  private Network() { } // Required by EF Core for materialization.

  public static Network Create(Guid siteId, Guid siteIntegrationId, string providerNetworkId, string name, bool isGuestNetwork)
  {
    ArgumentOutOfRangeException.ThrowIfEqual(siteId, Guid.Empty);
    ArgumentOutOfRangeException.ThrowIfEqual(siteIntegrationId, Guid.Empty);
    ArgumentException.ThrowIfNullOrWhiteSpace(providerNetworkId);
    ArgumentException.ThrowIfNullOrWhiteSpace(name);

    return new Network
    {
      Id = Guid.CreateVersion7(),
      SiteId = siteId,
      SiteIntegrationId = siteIntegrationId,
      ProviderNetworkId = providerNetworkId,
      Name = name,
      IsGuestNetwork = isGuestNetwork,
      IsEnabled = true,
    };
  }
}