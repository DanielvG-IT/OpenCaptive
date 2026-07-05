using OpenCaptive.Application.Common;

namespace OpenCaptive.Application.Portals;

public interface IPortalService
{
  // A Network has exactly one Portal — no list, no id in the route.
  Task<Result<PortalDto>> GetAsync(Guid networkId, CancellationToken cancellationToken = default);
  Task<Result<PortalDto>> CreateAsync(Guid networkId, CreatePortalInput input, CancellationToken cancellationToken = default);
  Task<Result<PortalDto>> UpdateAsync(Guid networkId, UpdatePortalInput input, CancellationToken cancellationToken = default);

  // PortalVersions are immutable once created — there is no update, only publish.
  Task<Result<List<PortalVersionDto>>> GetVersionsAsync(Guid portalId, CancellationToken cancellationToken = default);
  Task<Result<PortalVersionDto>> CreateVersionAsync(Guid portalId, CreatePortalVersionInput input, CancellationToken cancellationToken = default);
  Task<Result<PortalDto>> PublishVersionAsync(Guid portalId, Guid versionId, CancellationToken cancellationToken = default);
}
