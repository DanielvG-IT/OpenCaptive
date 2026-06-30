using Microsoft.EntityFrameworkCore;
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

  public Task<Result<OrganizationDto>> UpdateAsync(Guid id, UpdateOrganizationInput input, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }

  public Task<Result<OrganizationDto>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }

  public Task<Result<List<MemberDto>>> GetMembersAsync(Guid id, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }

  public Task<Result<OrganizationDto>> AddMemberAsync(Guid id, AddMemberInput input, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }

  public Task<Result<OrganizationDto>> RemoveMemberAsync(Guid id, Guid memberId, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }

  private static OrganizationDto ToDto(Organization organization) => new(organization.Id, organization.Name, organization.Slug);
}
