namespace OpenCaptive.Application.Auth.Models;

public sealed record LoginInput(string Email, string Password);
public sealed record RefreshInput(string RefreshToken);
public sealed record RegisterInput(string OrganizationName, string OrganizationSlug, string FirstName, string LastName, string Email, string Password);

public sealed record VerifyMfaInput(string ChallengeToken, string Code);
public sealed record RedeemRecoveryCodeInput(string ChallengeToken, string RecoveryCode);

public sealed record VerifyEmailInput(Guid UserId, string Token);
public sealed record ResendVerifyEmailInput(string Email);

public sealed record ForgotPasswordInput(string Email);
public sealed record ResetPasswordInput(Guid UserId, string Token, string NewPassword);


public sealed record LoginResponse(LoginStatus Status, TokenResponse? Tokens, string? ChallengeToken);
public sealed record TokenResponse(string AccessToken, DateTimeOffset AccessTokenExpiresAt, string RefreshToken, DateTimeOffset RefreshTokenExpiresAt);
public sealed record RegisterResponse(bool VerificationEmailSent);
public sealed record RedeemRecoveryCodeResponse(TokenResponse Tokens, int? RemainingCodesLeft);

public enum LoginStatus
{
  Success = 1,
  MfaRequired = 2,
  EmailVerificationRequired = 3,
  PasswordResetRequired = 4,
  AccountLocked = 5
}
