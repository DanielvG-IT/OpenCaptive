using OpenCaptive.Application.Auth.Models;

namespace OpenCaptive.Application.Auth.Contracts;

public interface IAccessTokenGenerator
{
  AccessTokenResponse Generate(AccessTokenRequest request);
}
