using OpenCaptive.Application.Common;
using OpenCaptive.Application.Organizations.Models;

namespace OpenCaptive.Application.Organizations.Contracts;

public interface IOrganizationService
{
  Task<Result<OrganizationDto>> GetAsync(CancellationToken cancellationToken = default);
  Task<Result<OrganizationDto>> UpdateAsync(UpdateOrganizationInput input, CancellationToken cancellationToken = default);
  Task<Result<OrganizationDto>> DeleteAsync(CancellationToken cancellationToken = default);

  Task<Result<List<MemberDto>>> GetMembersAsync(CancellationToken cancellationToken = default);
  Task<Result<OrganizationDto>> UpdateMemberAsync(UpdateMemberInput input, CancellationToken cancellationToken = default);
  Task<Result<OrganizationDto>> RemoveMemberAsync(Guid memberId, CancellationToken cancellationToken = default);
}
