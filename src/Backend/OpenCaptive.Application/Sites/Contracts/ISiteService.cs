using OpenCaptive.Application.Common;
using OpenCaptive.Application.Sites.Models;

namespace OpenCaptive.Application.Sites.Contracts;

public interface ISiteService
{
  Task<Result<SiteDto>> CreateAsync(CreateSiteInput input, CancellationToken cancellationToken = default);
  Task<Result<SiteDto>> GetOneAsync(Guid siteId, CancellationToken cancellationToken = default);
  Task<Result<List<SiteDto>>> GetAllAsync(CancellationToken cancellationToken = default);
  Task<Result<SiteDto>> UpdateAsync(Guid siteId, UpdateSiteInput input, CancellationToken cancellationToken = default);
  Task<Result> DeleteAsync(Guid siteId, CancellationToken cancellationToken = default);
}