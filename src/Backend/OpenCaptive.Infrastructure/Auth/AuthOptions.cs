namespace OpenCaptive.Infrastructure.Auth;

public sealed class JwtOptions
{
  public string SigningKey { get; init; } = string.Empty;
  public string Issuer { get; init; } = string.Empty;
  public string Audience { get; init; } = string.Empty;
  public TimeSpan Lifetime { get; init; }
  public TimeSpan ClockSkew { get; init; }
}

public sealed class RefreshTokenOptions
{
  public TimeSpan Lifetime { get; init; }
}

public sealed class EmailVerificationOptions
{
  public TimeSpan TokenLifetime { get; init; }
}

public sealed class PasswordResetOptions
{
  public TimeSpan TokenLifetime { get; init; }
}

public sealed class TwoFactorAuthenticationOptions
{
  public string AuthenticatorIssuer { get; init; } = "OpenCaptive";
}
