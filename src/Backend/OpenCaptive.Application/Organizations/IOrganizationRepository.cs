using OpenCaptive.Application.Common;
using OpenCaptive.Domain.Organizations;

namespace OpenCaptive.Application.Organizations;

public interface IOrganizationRepository
{
  Task<Organization?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
  Task<Organization?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
  Task<bool> ExistsBySlugAsync(string slug, CancellationToken cancellationToken = default);
  Task<Result> AddAsync(Organization organization, CancellationToken cancellationToken = default);
}
