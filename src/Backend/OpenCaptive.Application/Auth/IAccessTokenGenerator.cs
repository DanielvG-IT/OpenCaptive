namespace OpenCaptive.Application.Auth;

public interface IAccessTokenGenerator
{
  AccessTokenResponse Generate(AccessTokenRequest request);
}