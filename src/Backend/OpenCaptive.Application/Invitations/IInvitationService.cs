using OpenCaptive.Application.Common;

namespace OpenCaptive.Application.Invitations;

public interface IInvitationService
{
  // Admin side — scoped to the caller's organization via ICurrentUser, same as IOrganizationService.
  Task<Result<List<InvitationDto>>> GetAllAsync(CancellationToken cancellationToken = default);
  Task<Result<InvitationDto>> GetOneByIdAsync(Guid invitationId, CancellationToken cancellationToken = default);
  Task<Result<InvitationDto>> CreateAsync(CreateInvitationInput input, CancellationToken cancellationToken = default);
  Task<Result> RevokeAsync(Guid invitationId, CancellationToken cancellationToken = default);
  Task<Result<InvitationDto>> ResendAsync(Guid invitationId, CancellationToken cancellationToken = default);

  // Invitee side — identified by invitation id/token alone; the caller isn't a member yet,
  // so these must not assume ICurrentUser.OrganizationId.
  Task<Result<InvitationDto>> GetForInviteeAsync(Guid invitationId, CancellationToken cancellationToken = default);
  Task<Result> AcceptAsync(Guid invitationId, CancellationToken cancellationToken = default);
}
