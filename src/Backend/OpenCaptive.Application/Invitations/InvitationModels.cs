using OpenCaptive.Domain.Organizations;

namespace OpenCaptive.Application.Invitations;

public sealed record CreateInvitationInput(string Email, OrganizationRole Role);

// Email/Role/OrganizationId are never client-supplied here — they come from the invitation
// itself. Only present these fields when the invitee has no account yet; omit (empty body)
// when accepting as an already-authenticated existing user.
public sealed record AcceptInvitationInput(string? FirstName, string? LastName, string? Password);

public sealed record InvitationDto(Guid Id, string Email, OrganizationRole Role, DateTimeOffset ExpiresAt, bool IsAccepted);
