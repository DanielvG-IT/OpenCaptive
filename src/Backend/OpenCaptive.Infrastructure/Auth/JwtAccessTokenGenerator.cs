using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using OpenCaptive.Application.Auth.Contracts;
using OpenCaptive.Domain.Auth;

namespace OpenCaptive.Infrastructure.Auth;

public sealed class JwtAccessTokenGenerator(IOptions<JwtOptions> jwtOptions) : IAccessTokenGenerator
{
  private readonly SigningCredentials _credentials = new(CreateSecurityKey(jwtOptions.Value.SigningKey), SecurityAlgorithms.HmacSha256);
  private readonly JwtOptions _jwtOptions = jwtOptions.Value;
  private readonly JsonWebTokenHandler _tokenHandler = new();

  // Access-token revocation is intentionally not persisted. The current design relies on
  // short-lived access tokens and refresh-token rotation/reuse detection.

  public AccessTokenResponse Generate(AccessTokenRequest request)
  {
    var expiresAt = DateTimeOffset.UtcNow.Add(_jwtOptions.Lifetime);
    var claims = new Dictionary<string, object>
        {
            { JwtRegisteredClaimNames.Sub, request.UserId.ToString() },
            { JwtRegisteredClaimNames.Email, request.Email },
            { JwtRegisteredClaimNames.Jti, Guid.CreateVersion7().ToString() }
        };

    if (request.OrganizationId.HasValue && request.OrganizationRole.HasValue)
    {
      claims.Add(OrganizationClaimTypes.OrganizationId, request.OrganizationId.Value.ToString());
      claims.Add(OrganizationClaimTypes.OrganizationRole, request.OrganizationRole.Value.ToString());
    }

    var tokenDescriptor = new SecurityTokenDescriptor
    {
      Claims = claims,
      Audience = _jwtOptions.Audience,
      Issuer = _jwtOptions.Issuer,
      Expires = expiresAt.UtcDateTime,
      SigningCredentials = _credentials
    };

    var token = _tokenHandler.CreateToken(tokenDescriptor);

    return new AccessTokenResponse(token, expiresAt);
  }

  private static SymmetricSecurityKey CreateSecurityKey(string base64Key)
  {
    var keyBytes = Convert.FromBase64String(base64Key);
    if (keyBytes.Length < 32)
    {
      throw new InvalidOperationException("JWT signing key must be at least 256 bits.");
    }

    return new SymmetricSecurityKey(keyBytes);
  }
}
