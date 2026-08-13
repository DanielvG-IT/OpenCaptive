namespace OpenCaptive.Integrations.Abstractions.Capabilities;

public sealed record IntegrationCapabilities(
    bool SupportsGuestAccess,
    bool SupportsNetworks,
    bool SupportsVouchers);