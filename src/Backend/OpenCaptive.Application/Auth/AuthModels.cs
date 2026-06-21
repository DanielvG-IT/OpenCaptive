using OpenCaptive.Domain.Organizations;

namespace OpenCaptive.Application.Auth;

public sealed record LoginInput(string Email, string Password);
public sealed record RefreshInput(string RefreshToken);
public sealed record RegisterInput(string Email, string Password);
public sealed record VerifyMfaInput(string ChallengeToken, string Code);

public sealed record LoginResponse(LoginStatus Status, AuthResponse? Tokens, string? ChallengeToken);
public sealed record AuthResponse(string AccessToken, DateTimeOffset AccessTokenExpiresAt, string RefreshToken, DateTimeOffset RefreshTokenExpiresAt);

public sealed record MeInput(Guid UserId);
public sealed record MeResponse(Guid Id, string Email, bool EmailConfirmed, bool TwoFactorEnabled);

public sealed record AccessTokenRequest(Guid UserId, string Email, Guid? OrganizationId, OrganizationRole? OrganizationRole);
public sealed record AccessTokenResponse(string Token, DateTimeOffset ExpiresAt);


public enum LoginStatus
{
    Success = 1,
    MfaRequired = 2,
    EmailVerificationRequired = 3
}