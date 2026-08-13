namespace OpenCaptive.Integrations.Abstractions.Models;

public sealed record Voucher(
    string ProviderVoucherId,
    string Code,
    DateTimeOffset? ExpiresAt);