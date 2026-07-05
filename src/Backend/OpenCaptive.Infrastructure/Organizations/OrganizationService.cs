using Microsoft.EntityFrameworkCore;
using OpenCaptive.Application.Common;
using OpenCaptive.Application.Common.Contracts;
using OpenCaptive.Application.Organizations.Contracts;
using OpenCaptive.Application.Organizations.Errors;
using OpenCaptive.Application.Organizations.Models;
using OpenCaptive.Domain.Organizations;
using OpenCaptive.Infrastructure.Persistence;

namespace OpenCaptive.Infrastructure.Organizations;

public sealed class OrganizationService(OpenCaptiveDbContext dbContext, ICurrentUser currentUser) : IOrganizationService
{
  private readonly OpenCaptiveDbContext _dbContext = dbContext;
  private readonly ICurrentUser _currentUser = currentUser;

  public async Task<Result<OrganizationDto>> GetAsync(CancellationToken cancellationToken = default)
  {
    var currentOrgId = _currentUser.OrganizationId;

    var organization = await _dbContext.Organizations.FirstOrDefaultAsync(x => x.Id == currentOrgId, cancellationToken);
    if (organization is null)
    {
      return Result.Failure<OrganizationDto>(OrganizationErrors.NotFound(currentOrgId));
    }

    return Result.Success(ToDto(organization));
  }

  public Task<Result<OrganizationDto>> UpdateAsync(UpdateOrganizationInput input, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }

  public Task<Result<OrganizationDto>> DeleteAsync(CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }

  public Task<Result<List<MemberDto>>> GetMembersAsync(CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }

  public Task<Result<OrganizationDto>> UpdateMemberAsync(UpdateMemberInput input, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }

  public Task<Result<OrganizationDto>> RemoveMemberAsync(Guid memberId, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }

  private static OrganizationDto ToDto(Organization organization) => new(organization.Id, organization.Name, organization.Slug);
}
