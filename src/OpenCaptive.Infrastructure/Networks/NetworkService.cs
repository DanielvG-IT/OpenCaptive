using OpenCaptive.Application.Common;
using OpenCaptive.Application.Common.Contracts;
using OpenCaptive.Application.Networks;
using OpenCaptive.Infrastructure.Persistence;

namespace OpenCaptive.Infrastructure.Networks;

public sealed class NetworkService(OpenCaptiveDbContext dbContext, ICurrentUser currentUser) : INetworkService
{
  private readonly OpenCaptiveDbContext _dbContext = dbContext;
  private readonly ICurrentUser _currentUser = currentUser;

  // Must also persist the Network's initial Portal (Portal.Create(network.Id, ...)) in the
  // same transaction — a Portal has no independent creation path (see PortalEndpoints), so
  // this is the only place one is ever provisioned. Deliberately not calling IPortalService
  // here: with no other creation entrypoint left, there's nothing to share/duplicate against,
  // so this is one aggregate-consistency transaction, not cross-service coupling.
  public Task<Result<NetworkDto>> CreateAsync(Guid siteId, CreateNetworkInput input, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }

  public Task<Result<NetworkDto>> GetOneByIdAsync(Guid siteId, Guid networkId, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }

  public Task<Result<List<NetworkDto>>> GetAllAsync(Guid siteId, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }

  public Task<Result<NetworkDto>> UpdateAsync(Guid siteId, Guid networkId, UpdateNetworkInput input, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }

  public Task<Result> DeleteAsync(Guid siteId, Guid networkId, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }
}
