using OpenCaptive.Application.Common;
using OpenCaptive.Application.Common.Contracts;
using OpenCaptive.Application.Portals;
using OpenCaptive.Infrastructure.Persistence;

namespace OpenCaptive.Infrastructure.Portals;

public sealed class PortalService(OpenCaptiveDbContext dbContext, ICurrentUser currentUser) : IPortalService
{
  private readonly OpenCaptiveDbContext _dbContext = dbContext;
  private readonly ICurrentUser _currentUser = currentUser;

  public Task<Result<PortalDto>> GetAsync(Guid networkId, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }

  public Task<Result<PortalDto>> UpdateAsync(Guid networkId, UpdatePortalInput input, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }

  public Task<Result<List<PortalVersionDto>>> GetVersionsAsync(Guid portalId, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }

  public Task<Result<PortalVersionDto>> GetVersionAsync(Guid portalId, Guid versionId, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }

  public Task<Result<PortalVersionDto>> CreateVersionAsync(Guid portalId, CreatePortalVersionInput input, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }

  public Task<Result<PortalDto>> PublishVersionAsync(Guid portalId, Guid versionId, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }

  public Task<Result> DeleteVersionAsync(Guid portalId, Guid versionId, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }
}
