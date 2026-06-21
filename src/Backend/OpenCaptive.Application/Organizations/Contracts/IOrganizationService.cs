using OpenCaptive.Application.Common;
using OpenCaptive.Application.Organizations.Models;

namespace OpenCaptive.Application.Organizations.Contracts;

public interface IOrganizationService
{
  Task<Result<OrganizationDto>> GetAsync(Guid id, CancellationToken cancellationToken = default);
  Task<Result<OrganizationDto>> CreateAsync(CreateOrganizationInput input, CancellationToken cancellationToken = default);
}
