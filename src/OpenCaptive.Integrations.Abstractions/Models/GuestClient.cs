namespace OpenCaptive.Integrations.Abstractions.Models;

public sealed record GuestClient(
    string ProviderClientId,
    string MacAddress,
    string? Hostname);