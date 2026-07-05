namespace OpenCaptive.Application.Networks;

// Network.Create requires a SiteIntegrationId + ProviderNetworkId (see Domain.Networks.Network) —
// networks are synchronized from an integration, not freely named by an admin. Confirm this
// input shape still matches once the sync flow is designed; it may end up populated by
// SiteIntegrationService rather than a directly-authored request body.
public sealed record CreateNetworkInput(Guid SiteIntegrationId, string ProviderNetworkId, string Name, bool IsGuestNetwork);
public sealed record UpdateNetworkInput(string? Name, bool? IsGuestNetwork, bool? IsEnabled);

public sealed record NetworkDto(Guid Id, Guid SiteId, Guid SiteIntegrationId, string Name, string ProviderNetworkId, bool IsGuestNetwork, bool IsEnabled);
