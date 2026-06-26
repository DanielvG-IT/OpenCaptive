namespace OpenCaptive.Infrastructure.Auth;

public sealed class JwtOptions
{
  public const string SectionName = "Authentication:Jwt";

  public required string SigningKey { get; init; }
  public required string Issuer { get; init; }
  public required string Audience { get; init; }
  public required TimeSpan Lifetime { get; init; }
  public required TimeSpan ClockSkew { get; init; }
}

public sealed class RefreshTokenOptions
{
  public const string SectionName = "Authentication:RefreshToken";

  public TimeSpan Lifetime { get; init; }
}

public sealed class EmailVerificationOptions
{
  public const string SectionName = "Authentication:EmailVerification";

  public required TimeSpan TokenLifetime { get; init; }
  public required bool RequireBeforeLogin { get; init; }
  public required TimeSpan ResendCooldown { get; init; }
  public required int MaxResendAttempts { get; init; }
}

public sealed class PasswordResetOptions
{
  public const string SectionName = "Authentication:PasswordReset";

  public TimeSpan TokenLifetime { get; init; }
}

public sealed class TwoFactorAuthenticationOptions
{
  public const string SectionName = "Authentication:TwoFactorAuthentication";

  public string AuthenticatorIssuer { get; init; } = "OpenCaptive";
}
