using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using OpenCaptive.Application.Email.Contracts;

namespace OpenCaptive.Infrastructure.Frontend;

public sealed class FrontendLinkFactory(IOptions<FrontendOptions> options) : IFrontendLinkFactory
{
  private readonly FrontendOptions _options = options.Value;

  public string CreateInvitationLink(Guid invitationId, string token) =>
      CreateLink(FrontendRoutes.Invitation, new Dictionary<string, string?>
      {
        ["invitationId"] = invitationId.ToString(),
        ["token"] = token
      });

  public string CreateResetPasswordLink(Guid userId, string token) =>
      CreateLink(FrontendRoutes.ResetPassword, new Dictionary<string, string?>
      {
        ["userId"] = userId.ToString(),
        ["token"] = token
      });

  public string CreateVerifyEmailLink(Guid userId, string token) =>
      CreateLink(FrontendRoutes.VerifyEmail, new Dictionary<string, string?>
      {
        ["userId"] = userId.ToString(),
        ["token"] = token
      });

  private string CreateLink(string path, IReadOnlyDictionary<string, string?> queryParameters)
  {
    var builder = new UriBuilder(_options.ApplicationUrl)
    {
      Path = path
    };

    return QueryHelpers.AddQueryString(builder.Uri.ToString(), queryParameters);
  }
}