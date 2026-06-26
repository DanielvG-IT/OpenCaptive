using OpenCaptive.Domain.Organizations;

namespace OpenCaptive.Application.Auth.Models;

public sealed record LoginInput(string Email, string Password);
public sealed record RefreshInput(string RefreshToken);
public sealed record RegisterInput(string OrganizationName, string OrganizationSlug, string FirstName, string LastName, string Email, string Password);
public sealed record VerifyMfaInput(string ChallengeToken, string Code);

public sealed record LoginResponse(LoginStatus Status, TokenResponse? Tokens, string? ChallengeToken);
public sealed record TokenResponse(string AccessToken, DateTimeOffset AccessTokenExpiresAt, string RefreshToken, DateTimeOffset RefreshTokenExpiresAt);
public sealed record RegisterResponse(bool VerificationEmailSent);
public sealed record MeResponse(Guid Id, string Email, string FirstName, string LastName, bool EmailConfirmed, bool TwoFactorEnabled);

public enum LoginStatus
{
  Success = 1,
  MfaRequired = 2,
  EmailVerificationRequired = 3,
  PasswordResetRequired = 4,
  AccountLocked = 5
}
