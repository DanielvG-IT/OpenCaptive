using OpenCaptive.Application.Common;
using OpenCaptive.Application.Organizations.Models;

namespace OpenCaptive.Application.Organizations.Contracts;

public interface IOrganizationService
{
  Task<Result<OrganizationDto>> GetAsync(Guid id, CancellationToken cancellationToken = default);
  Task<Result<OrganizationDto>> UpdateAsync(Guid id, UpdateOrganizationInput input, CancellationToken cancellationToken = default);
  Task<Result<OrganizationDto>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

  Task<Result<List<MemberDto>>> GetMembersAsync(Guid id, CancellationToken cancellationToken = default);
  Task<Result<OrganizationDto>> AddMemberAsync(Guid id, AddMemberInput input, CancellationToken cancellationToken = default);
  Task<Result<OrganizationDto>> RemoveMemberAsync(Guid id, Guid memberId, CancellationToken cancellationToken = default);
}
