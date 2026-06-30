using OpenCaptive.Application.Common;
using OpenCaptive.Application.Common.Contracts;
using OpenCaptive.Application.Sites.Contracts;
using OpenCaptive.Application.Sites.Models;
using OpenCaptive.Infrastructure.Persistence;

namespace OpenCaptive.Infrastructure.Sites;

public sealed class SiteService(OpenCaptiveDbContext dbContext, ICurrentUser currentUser) : ISiteService
{
  private readonly OpenCaptiveDbContext _dbContext = dbContext;
  private readonly ICurrentUser _currentUser = currentUser;

  public Task<Result<SiteDto>> CreateAsync(CreateSiteInput input, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }

  public Task<Result<List<SiteDto>>> GetAllAsync(CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }

  public Task<Result<SiteDto>> GetOneAsync(Guid siteId, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }

  public Task<Result<SiteDto>> UpdateAsync(Guid siteId, UpdateSiteInput input, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }

  public Task<Result> DeleteAsync(Guid siteId, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }
}
