namespace OpenCaptive.Integrations.UniFi.Models;

public sealed record UniFiNetwork(Guid Id, string Name, string Ssid, bool Enabled);