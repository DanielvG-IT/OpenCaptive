using OpenCaptive.Application.Common;
using OpenCaptive.Application.Common.Contracts;
using OpenCaptive.Application.Networks;
using OpenCaptive.Infrastructure.Persistence;

namespace OpenCaptive.Infrastructure.Networks;

public sealed class NetworkService(OpenCaptiveDbContext dbContext, ICurrentUser currentUser) : INetworkService
{
  private readonly OpenCaptiveDbContext _dbContext = dbContext;
  private readonly ICurrentUser _currentUser = currentUser;

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
