using OpenCaptive.Application.Common;

namespace OpenCaptive.Application.Sites;

public interface ISiteService
{
  Task<Result<SiteDto>> CreateAsync(CreateSiteInput input, CancellationToken cancellationToken = default);
  Task<Result<SiteDto>> GetOneByIdAsync(Guid siteId, CancellationToken cancellationToken = default);
  Task<Result<List<SiteSummaryDto>>> GetAllAsync(CancellationToken cancellationToken = default);
  Task<Result<SiteDto>> UpdateAsync(Guid siteId, UpdateSiteInput input, CancellationToken cancellationToken = default);
  Task<Result> DeleteAsync(Guid siteId, CancellationToken cancellationToken = default);
}