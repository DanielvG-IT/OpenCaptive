using OpenCaptive.Domain.Organizations;

namespace OpenCaptive.Application.Invitations;

public sealed record CreateInvitationInput(string Email, OrganizationRole Role);

public sealed record InvitationDto(Guid Id, string Email, OrganizationRole Role, DateTimeOffset ExpiresAt, bool IsAccepted);
