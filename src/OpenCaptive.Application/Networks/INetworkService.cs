using OpenCaptive.Application.Common;

namespace OpenCaptive.Application.Networks;

public interface INetworkService
{
  Task<Result<NetworkDto>> CreateAsync(Guid siteId, CreateNetworkInput input, CancellationToken cancellationToken = default);
  Task<Result<NetworkDto>> GetOneByIdAsync(Guid siteId, Guid networkId, CancellationToken cancellationToken = default);
  Task<Result<List<NetworkDto>>> GetAllAsync(Guid siteId, CancellationToken cancellationToken = default);
  Task<Result<NetworkDto>> UpdateAsync(Guid siteId, Guid networkId, UpdateNetworkInput input, CancellationToken cancellationToken = default);
  Task<Result> DeleteAsync(Guid siteId, Guid networkId, CancellationToken cancellationToken = default);
}
