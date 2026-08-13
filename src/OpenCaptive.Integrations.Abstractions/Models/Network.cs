namespace OpenCaptive.Integrations.Abstractions.Models;

public sealed record Network(
    string ProviderNetworkId,
    string Name,
    string Ssid,
    bool Enabled);