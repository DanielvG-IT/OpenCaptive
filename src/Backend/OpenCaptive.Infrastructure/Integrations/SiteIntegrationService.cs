using OpenCaptive.Application.Common;
using OpenCaptive.Application.Common.Contracts;
using OpenCaptive.Application.Integrations;
using OpenCaptive.Infrastructure.Persistence;

namespace OpenCaptive.Infrastructure.Integrations;

public sealed class SiteIntegrationService(OpenCaptiveDbContext dbContext, ICurrentUser currentUser) : ISiteIntegrationService
{
  private readonly OpenCaptiveDbContext _dbContext = dbContext;
  private readonly ICurrentUser _currentUser = currentUser;

  public Task<Result<SiteIntegrationDto>> CreateAsync(Guid siteId, CreateSiteIntegrationInput input, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }

  public Task<Result<SiteIntegrationDto>> GetOneByIdAsync(Guid siteId, Guid integrationId, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }

  public Task<Result<List<SiteIntegrationDto>>> GetAllAsync(Guid siteId, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }

  public Task<Result<SiteIntegrationDto>> UpdateAsync(Guid siteId, Guid integrationId, UpdateSiteIntegrationInput input, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }

  public Task<Result> DeleteAsync(Guid siteId, Guid integrationId, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }

  public Task<Result<SiteIntegrationDto>> ConnectAsync(Guid siteId, Guid integrationId, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }

  public Task<Result<SiteIntegrationDto>> DisconnectAsync(Guid siteId, Guid integrationId, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }

  public Task<Result<SiteIntegrationDto>> SyncAsync(Guid siteId, Guid integrationId, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }
}
