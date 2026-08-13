using OpenCaptive.Integrations.Abstractions.Models;

namespace OpenCaptive.Integrations.Abstractions.Vouchers;

public interface IVoucherIntegration
{
  Task<IReadOnlyList<Voucher>> GetVouchersAsync(CancellationToken cancellationToken = default);
  Task<Voucher> CreateVoucherAsync(TimeSpan duration, CancellationToken cancellationToken = default);
  Task<bool> DeleteVoucherAsync(string providerVoucherId, CancellationToken cancellationToken = default);
}