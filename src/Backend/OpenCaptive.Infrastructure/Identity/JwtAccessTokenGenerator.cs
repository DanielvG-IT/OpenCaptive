using Microsoft.Extensions.Options;
using OpenCaptive.Application.Auth;
using OpenCaptive.Domain.Auth;
using OpenCaptive.Infrastructure.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace OpenCaptive.Infrastructure.Identity;

public sealed class JwtAccessTokenGenerator(IOptions<JwtOptions> jwtOptions) : IAccessTokenGenerator
{
  private readonly JwtOptions _jwtOptions = jwtOptions.Value;

  // TODO(security, deferred): Persist JTI when access-token revocation,
  // forced logout, or token blacklisting becomes a requirement. Current
  // design relies on short-lived access tokens (15m) and refresh-token
  // rotation/reuse detection instead of maintaining an access-token
  // revocation store.

  public AccessTokenResponse Generate(AccessTokenRequest request)
  {
    var signingKey = Convert.FromBase64String(_jwtOptions.SigningKey);
    if (signingKey.Length < 32)
    {
      throw new InvalidOperationException("JWT signing key must be at least 256 bits.");
    }

    var credentials = new SigningCredentials(new SymmetricSecurityKey(signingKey), SecurityAlgorithms.HmacSha256);
    var expiresAt = DateTimeOffset.UtcNow.Add(_jwtOptions.Lifetime);

    var claims = new List<Claim>
    {
      new(JwtRegisteredClaimNames.Sub, request.UserId.ToString()),
      new(JwtRegisteredClaimNames.Email, request.Email),
      new(JwtRegisteredClaimNames.Jti, Guid.CreateVersion7().ToString())
    };

    if (request.OrganizationId is not null && request.OrganizationRole is not null)
    {
      claims.Add(new Claim(OrganizationClaimTypes.OrganizationId, request.OrganizationId.Value.ToString()));
      claims.Add(new Claim(OrganizationClaimTypes.OrganizationRole, request.OrganizationRole.Value.ToString()));
    }

    var tokenDescriptor = new SecurityTokenDescriptor
    {
      Subject = new ClaimsIdentity(claims),
      Audience = _jwtOptions.Audience,
      Issuer = _jwtOptions.Issuer,
      SigningCredentials = credentials,
      Expires = expiresAt.UtcDateTime,
    };

    var tokenHandler = new JsonWebTokenHandler();
    var tokenString = tokenHandler.CreateToken(tokenDescriptor);

    return new AccessTokenResponse(tokenString, expiresAt);
  }
}