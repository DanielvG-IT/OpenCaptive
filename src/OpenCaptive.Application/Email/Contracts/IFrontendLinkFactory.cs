namespace OpenCaptive.Application.Email.Contracts;

public interface IFrontendLinkFactory
{
  string CreateVerifyEmailLink(Guid userId, string token);

  string CreateResetPasswordLink(Guid userId, string token);

  string CreateInvitationLink(Guid invitationId, string token);
}