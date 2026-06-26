using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using OpenCaptive.Application.Email.Contracts;

namespace OpenCaptive.Infrastructure.Frontend;

public sealed class FrontendLinkFactory(IOptions<FrontendOptions> options) : IFrontendLinkFactory
{
  private readonly FrontendOptions _options = options.Value;

  public string CreateInvitationLink(Guid invitationId, string token)
  {
    throw new NotImplementedException();
  }

  public string CreateResetPasswordLink(Guid userId, string token)
  {
    throw new NotImplementedException();
  }

  public string CreateVerifyEmailLink(Guid userId, string token)
  {
    var builder = new UriBuilder(_options.ApplicationUrl)
    {
      Path = FrontendRoutes.VerifyEmail
    };

    return QueryHelpers.AddQueryString(
        builder.Uri.ToString(),
        new Dictionary<string, string?>
        {
          ["userId"] = userId.ToString(),
          ["token"] = token
        });
  }
}