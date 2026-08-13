namespace OpenCaptive.Integrations.UniFi.Models;

public sealed record UniFiGuestClient(Guid Id, string MacAddress, string? Hostname);