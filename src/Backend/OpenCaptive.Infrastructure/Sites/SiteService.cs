using Microsoft.EntityFrameworkCore;
using Npgsql;
using OpenCaptive.Application.Common;
using OpenCaptive.Application.Common.Contracts;
using OpenCaptive.Application.Sites;
using OpenCaptive.Domain.Sites;
using OpenCaptive.Infrastructure.Persistence;

namespace OpenCaptive.Infrastructure.Sites;

public sealed class SiteService(OpenCaptiveDbContext dbContext, ICurrentUser currentUser) : ISiteService
{
  private readonly OpenCaptiveDbContext _dbContext = dbContext;
  private readonly ICurrentUser _currentUser = currentUser;

  public async Task<Result<SiteDto>> CreateAsync(CreateSiteInput input, CancellationToken cancellationToken = default)
  {
    var currentOrgId = _currentUser.OrganizationId;

    try
    {
      var exists = await _dbContext.Sites.AnyAsync(s => s.Slug == input.Slug && s.OrganizationId == currentOrgId, cancellationToken);
      if (exists)
      {
        return Result.Failure<SiteDto>(SiteErrors.SlugAlreadyExists(input.Slug));
      }

      var site = Site.Create(currentOrgId, input.Name, input.Slug, input.TimeZone, input.Description);
      _dbContext.Sites.Add(site);
      await _dbContext.SaveChangesAsync(cancellationToken);

      return Result.Success(ToDto(site));
    }
    catch (DbUpdateException dbEx) when (dbEx.InnerException is PostgresException pgEx && pgEx.SqlState == PostgresErrorCodes.UniqueViolation)
    {
      return Result.Failure<SiteDto>(SiteErrors.SlugAlreadyExists(input.Slug));
    }
  }

  public async Task<Result<List<SiteSummaryDto>>> GetAllAsync(CancellationToken cancellationToken = default)
  {
    var currentOrgId = _currentUser.OrganizationId;

    var sites = await _dbContext.Sites.Where(o => o.OrganizationId == currentOrgId).ToListAsync(cancellationToken);
    var dtos = sites.Select(ToSummaryDto).ToList();

    return Result.Success(dtos);
  }

  public async Task<Result<SiteDto>> GetOneByIdAsync(Guid siteId, CancellationToken cancellationToken = default)
  {
    var currentOrgId = _currentUser.OrganizationId;

    var site = await _dbContext.Sites.SingleOrDefaultAsync(x => x.Id == siteId && x.OrganizationId == currentOrgId, cancellationToken);
    if (site is null)
    {
      return Result.Failure<SiteDto>(SiteErrors.NotFound(siteId));
    }

    return Result.Success(ToDto(site));
  }

  public async Task<Result<SiteDto>> UpdateAsync(Guid id, UpdateSiteInput input, CancellationToken cancellationToken = default)
  {
    var currentOrgId = _currentUser.OrganizationId;

    try
    {
      var site = await _dbContext.Sites.FirstOrDefaultAsync(s => s.Id == id && s.OrganizationId == currentOrgId, cancellationToken);
      if (site is null)
      {
        return Result.Failure<SiteDto>(SiteErrors.NotFound(id));
      }

      if (!string.IsNullOrWhiteSpace(input.Slug) && input.Slug != site.Slug)
      {
        // CRITICAL: Exclude this site itself
        var slugExists = await _dbContext.Sites.AnyAsync(s => s.Slug == input.Slug && s.OrganizationId == currentOrgId && s.Id != id, cancellationToken);
        if (slugExists)
        {
          return Result.Failure<SiteDto>(SiteErrors.SlugAlreadyExists(input.Slug));
        }
      }

      if (!string.IsNullOrWhiteSpace(input.Name))
        site.UpdateName(input.Name);

      if (!string.IsNullOrWhiteSpace(input.Slug))
        site.UpdateSlug(input.Slug);

      if (!string.IsNullOrWhiteSpace(input.TimeZone))
        site.UpdateTimeZone(input.TimeZone);

      if (input.Description != site.Description)
        site.UpdateDescription(input.Description);

      await _dbContext.SaveChangesAsync(cancellationToken);

      return Result.Success(ToDto(site));
    }
    catch (DbUpdateException dbEx) when (dbEx.InnerException is PostgresException pgEx && pgEx.SqlState == PostgresErrorCodes.UniqueViolation)
    {
      return Result.Failure<SiteDto>(SiteErrors.SlugAlreadyExists(input.Slug ?? string.Empty));
    }
  }

  public async Task<Result> DeleteAsync(Guid siteId, CancellationToken cancellationToken = default)
  {
    var currentOrgId = _currentUser.OrganizationId;

    try
    {
      var deletedCount = await _dbContext.Sites.Where(x => x.Id == siteId && x.OrganizationId == currentOrgId).ExecuteDeleteAsync(cancellationToken);
      return deletedCount > 0
                  ? Result.Success()
                  : Result.Failure(SiteErrors.NotFound(siteId));
    }
    catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.ForeignKeyViolation)
    {
      return Result.Failure(SiteErrors.CannotDeleteWithActiveDependencies(siteId));
    }
  }

  private static SiteDto ToDto(Site x) => new(x.Id, x.Name, x.Slug, x.Description, x.TimeZone, x.IsEnabled);
  private static SiteSummaryDto ToSummaryDto(Site x) => new(x.Id, x.Name, x.Slug, x.IsEnabled);
}
