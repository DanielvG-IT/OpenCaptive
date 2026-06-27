using Microsoft.EntityFrameworkCore;
using Npgsql;
using OpenCaptive.Application.Common;
using OpenCaptive.Application.Organizations.Contracts;
using OpenCaptive.Application.Organizations.Errors;
using OpenCaptive.Application.Organizations.Models;
using OpenCaptive.Domain.Organizations;
using OpenCaptive.Infrastructure.Persistence;

namespace OpenCaptive.Infrastructure.Organizations;

public sealed class OrganizationService(OpenCaptiveDbContext dbContext) : IOrganizationService
{
  private readonly OpenCaptiveDbContext _dbContext = dbContext;

  public async Task<Result<OrganizationDto>> GetAsync(Guid id, CancellationToken cancellationToken = default)
  {
    var organization = await _dbContext.Organizations.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    if (organization is null)
    {
      return Result.Failure<OrganizationDto>(OrganizationErrors.NotFound(id));
    }

    return Result.Success(ToDto(organization));
  }

  public async Task<Result<OrganizationDto>> CreateAsync(CreateOrganizationInput input, CancellationToken cancellationToken = default)
  {
    if (await _dbContext.Organizations.AnyAsync(x => x.Slug == input.Slug, cancellationToken))
    {
      return Result.Failure<OrganizationDto>(OrganizationErrors.SlugAlreadyExists(input.Slug));
    }

    var organization = Organization.Create(input.Name, input.Slug);

    _dbContext.Organizations.Add(organization);

    try
    {
      await _dbContext.SaveChangesAsync(cancellationToken);
    }
    catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
    {
      return Result.Failure<OrganizationDto>(OrganizationErrors.SlugAlreadyExists(input.Slug));
    }

    return Result.Success(ToDto(organization));
  }

  private static OrganizationDto ToDto(Organization organization) => new(organization.Id, organization.Name, organization.Slug);
}
