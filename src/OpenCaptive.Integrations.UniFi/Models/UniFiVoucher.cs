namespace OpenCaptive.Integrations.UniFi.Models;

public sealed record UniFiVoucher(Guid Id, string Code, DateTimeOffset? ExpiresAt);