using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using OpenCaptive.Application.Auth.Contracts;

namespace OpenCaptive.Infrastructure.Auth;

public sealed class JwtTwoFactorTokenGenerator(IOptions<TwoFactorAuthenticationOptions> twoFactorOptions, IOptions<JwtOptions> jwtOptions) : ITwoFactorTokenGenerator
{
  private readonly SymmetricSecurityKey _securityKey = CreateSecurityKey(jwtOptions.Value.SigningKey);
  private readonly TwoFactorAuthenticationOptions _twoFactorOptions = twoFactorOptions.Value;
  private readonly JwtOptions _jwtOptions = jwtOptions.Value;
  private readonly JsonWebTokenHandler _tokenHandler = new();

  private const string TwoFactorClaimName = "purpose";
  private const string TwoFactorClaimValue = "mfa-challenge";

  // TODO(security, deferred): Combat JTI replay protection for challenge tokens

  public string Generate(Guid userId)
  {
    var expiresAt = DateTimeOffset.UtcNow.Add(_twoFactorOptions.ChallengeTokenLifetime);

    // Using a dictionary is cleaner for the newer SecurityTokenDescriptor
    var claims = new Dictionary<string, object>
        {
            { TwoFactorClaimName, TwoFactorClaimValue },
            { JwtRegisteredClaimNames.Sub, userId.ToString() },
            { JwtRegisteredClaimNames.Jti, Guid.CreateVersion7().ToString() }
        };

    var tokenDescriptor = new SecurityTokenDescriptor
    {
      Claims = claims,
      Audience = _jwtOptions.Audience,
      Issuer = _jwtOptions.Issuer,
      Expires = expiresAt.UtcDateTime,
      SigningCredentials = new SigningCredentials(_securityKey, SecurityAlgorithms.HmacSha256)
    };

    return _tokenHandler.CreateToken(tokenDescriptor);
  }

  public async Task<Guid?> TryGetUserId(string token)
  {
    var validationResult = await ValidateTokenAsync(token);
    if (!validationResult.IsValid)
    {
      return null;
    }

    var hasPurposeClaim = validationResult.ClaimsIdentity.HasClaim(TwoFactorClaimName, TwoFactorClaimValue);
    if (!hasPurposeClaim)
    {
      return null;
    }

    var subClaim = validationResult.ClaimsIdentity.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

    if (Guid.TryParse(subClaim, out var userId))
    {
      return userId;
    }

    return null;
  }

  private async Task<TokenValidationResult> ValidateTokenAsync(string token)
  {
    var validationParameters = new TokenValidationParameters
    {
      ValidateIssuerSigningKey = true,
      IssuerSigningKey = _securityKey,
      ValidateIssuer = true,
      ValidIssuer = _jwtOptions.Issuer,
      ValidateAudience = true,
      ValidAudience = _jwtOptions.Audience,
      ValidateLifetime = true,
      ClockSkew = TimeSpan.Zero
    };

    return await _tokenHandler.ValidateTokenAsync(token, validationParameters);
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