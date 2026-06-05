using Microsoft.EntityFrameworkCore;
using Npgsql;
using OpenCaptive.Application.Common;
using OpenCaptive.Application.Organizations;
using OpenCaptive.Domain.Organizations;
using OpenCaptive.Infrastructure.Persistence;

namespace OpenCaptive.Infrastructure.Repositories;

public sealed class OrganizationRepository(OpenCaptiveDbContext dbContext) : IOrganizationRepository
{
  private readonly OpenCaptiveDbContext _dbContext = dbContext;

  public async Task<Result> AddAsync(Organization organization, CancellationToken cancellationToken = default)
  {
    await _dbContext.Organizations.AddAsync(organization, cancellationToken);

    try
    {
      await _dbContext.SaveChangesAsync(cancellationToken);
      return Result.Success();
    }
    catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
    {
      // Safety net for the race between ExistsBySlugAsync and the insert.
      return Result.Failure(OrganizationErrors.SlugAlreadyExists(organization.Slug));
    }
  }

  public async Task<bool> ExistsBySlugAsync(string slug, CancellationToken cancellationToken = default)
  {
    return await _dbContext.Organizations.AnyAsync(x => x.Slug == slug, cancellationToken: cancellationToken);
  }

  public async Task<Organization?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
  {
    return await _dbContext.Organizations.FirstOrDefaultAsync(x => x.Id == id, cancellationToken: cancellationToken);
  }

  public async Task<Organization?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
  {
    return await _dbContext.Organizations.FirstOrDefaultAsync(x => x.Slug == slug, cancellationToken: cancellationToken);
  }
}
