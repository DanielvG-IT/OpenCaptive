using OpenCaptive.Application.Common;

namespace OpenCaptive.Application.Organizations;

public interface IOrganizationService
{
  Task<Result<OrganizationDto>> GetAsync(Guid id, CancellationToken cancellationToken = default);
  Task<Result<OrganizationDto>> CreateAsync(CreateOrganizationInput input, CancellationToken cancellationToken = default);
}
