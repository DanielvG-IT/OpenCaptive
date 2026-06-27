using OpenCaptive.Infrastructure.Common.Options;

namespace OpenCaptive.Infrastructure.Auth;

public sealed class JwtOptions : IOptionsSection
{
  public static string SectionName { get; } = "Authentication:Jwt";

  public required string SigningKey { get; init; }
  public required string Issuer { get; init; }
  public required string Audience { get; init; }
  public required TimeSpan Lifetime { get; init; }
  public required TimeSpan ClockSkew { get; init; }
}

public sealed class RefreshTokenOptions : IOptionsSection
{
  public static string SectionName { get; } = "Authentication:RefreshToken";

  public TimeSpan Lifetime { get; init; }
}

public sealed class EmailVerificationOptions : IOptionsSection
{
  public static string SectionName { get; } = "Authentication:EmailVerification";

  public required TimeSpan TokenLifetime { get; init; }
  public required bool RequireBeforeLogin { get; init; }
  public required TimeSpan ResendCooldown { get; init; }
  public required int MaxResendAttempts { get; init; }
}

public sealed class PasswordResetOptions : IOptionsSection
{
  public static string SectionName { get; } = "Authentication:PasswordReset";

  public TimeSpan TokenLifetime { get; init; }
}

public sealed class TwoFactorAuthenticationOptions : IOptionsSection
{
  public static string SectionName { get; } = "Authentication:TwoFactorAuthentication";

  public string AuthenticatorIssuer { get; init; } = "OpenCaptive";
  public TimeSpan ChallengeTokenLifetime { get; init; }
  public int RecoveryCodeCount { get; init; } = 10;
}
