using OpenCaptive.Application.Common;

namespace OpenCaptive.Application.Portals;

public interface IPortalService
{
  // A Network has exactly one Portal, auto-provisioned when the Network is created
  // (see NetworkService) — there is no manual create here, no list, no id in the route.
  Task<Result<PortalDto>> GetAsync(Guid networkId, CancellationToken cancellationToken = default);
  Task<Result<PortalDto>> UpdateAsync(Guid networkId, UpdatePortalInput input, CancellationToken cancellationToken = default);

  // PortalVersions are immutable once created — every save mints a new version, there is no
  // content update, only publish. A version that has ever been published can never be deleted
  // (see PortalErrors.CannotDeletePublishedVersion) — only undeleted drafts may be removed.
  Task<Result<List<PortalVersionDto>>> GetVersionsAsync(Guid portalId, CancellationToken cancellationToken = default);
  Task<Result<PortalVersionDto>> GetVersionAsync(Guid portalId, Guid versionId, CancellationToken cancellationToken = default);
  Task<Result<PortalVersionDto>> CreateVersionAsync(Guid portalId, CreatePortalVersionInput input, CancellationToken cancellationToken = default);
  Task<Result<PortalDto>> PublishVersionAsync(Guid portalId, Guid versionId, CancellationToken cancellationToken = default);
  Task<Result> DeleteVersionAsync(Guid portalId, Guid versionId, CancellationToken cancellationToken = default);
}
