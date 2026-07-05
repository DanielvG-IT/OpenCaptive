using OpenCaptive.Application.Common;

namespace OpenCaptive.Application.Invitations;

public static class InvitationErrors
{
  public static Error NotFound(Guid id) =>
    Error.NotFound("Invitation.NotFound", $"No invitation was found with id '{id}'.");

  public static Error AlreadyAccepted(Guid id) =>
    Error.Conflict("Invitation.AlreadyAccepted", $"Invitation '{id}' has already been accepted.");

  public static Error Expired(Guid id) =>
    Error.Conflict("Invitation.Expired", $"Invitation '{id}' has expired.");

  public static Error AlreadyPendingForEmail(string email) =>
    Error.Conflict("Invitation.AlreadyPending", $"An invitation is already pending for '{email}'.");
}
