using OpenCaptive.Application.Common;

namespace OpenCaptive.Application.Integrations;

public interface ISiteIntegrationService
{
  Task<Result<SiteIntegrationDto>> CreateAsync(Guid siteId, CreateSiteIntegrationInput input, CancellationToken cancellationToken = default);
  Task<Result<SiteIntegrationDto>> GetOneByIdAsync(Guid siteId, Guid integrationId, CancellationToken cancellationToken = default);
  Task<Result<List<SiteIntegrationDto>>> GetAllAsync(Guid siteId, CancellationToken cancellationToken = default);
  Task<Result<SiteIntegrationDto>> UpdateAsync(Guid siteId, Guid integrationId, UpdateSiteIntegrationInput input, CancellationToken cancellationToken = default);
  Task<Result> DeleteAsync(Guid siteId, Guid integrationId, CancellationToken cancellationToken = default);

  // Vendor-facing actions — Integration is the boundary that talks to the controller;
  // Application never branches on which vendor (UniFi/Omada/MikroTik) is behind it.
  // SyncNetworks and RefreshCapabilities are deliberately separate: the Domain entity already
  // tracks them independently (LastSuccessfulConnectionAt vs LastCapabilityRefreshAt /
  // MarkCapabilitiesRefreshed()) — pulling the network/SSID list from the vendor is not the
  // same operation as refreshing what the vendor's API can do.
  Task<Result<SiteIntegrationDto>> ConnectAsync(Guid siteId, Guid integrationId, CancellationToken cancellationToken = default);
  Task<Result<SiteIntegrationDto>> DisconnectAsync(Guid siteId, Guid integrationId, CancellationToken cancellationToken = default);
  Task<Result<SiteIntegrationDto>> SyncNetworksAsync(Guid siteId, Guid integrationId, CancellationToken cancellationToken = default);
  Task<Result<SiteIntegrationDto>> RefreshCapabilitiesAsync(Guid siteId, Guid integrationId, CancellationToken cancellationToken = default);
}
