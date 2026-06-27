using OpenCaptive.Application.Common;

namespace OpenCaptive.Application.Auth.Errors;

public static class AuthErrors
{
    public static readonly Error InvalidCredentials =
        Error.Unauthorized("auth.invalid_credentials", "Invalid email or password.");

    public static readonly Error UserAlreadyExists =
        Error.Conflict("auth.user_already_exists", "A user with this email already exists.");

    public static readonly Error InvalidRefreshToken =
        Error.Unauthorized("auth.invalid_refresh_token", "The refresh token is invalid.");

    public static readonly Error RefreshTokenExpired =
        Error.Unauthorized("auth.refresh_token_expired", "The refresh token has expired.");

    public static readonly Error UserNotFound =
        Error.NotFound("auth.user_not_found", "The user could not be found.");

    public static readonly Error UserInactive =
        Error.Forbidden("auth.user_inactive", "The user account is inactive.");

    public static readonly Error UserCreationFailed =
        Error.Failure("auth.user_creation_failed", "Account could not be created. Please try again.");

    public static readonly Error InvalidEmailVerificationToken =
        Error.Unauthorized("auth.invalid_email_verification_token", "The email verification link is invalid or has expired.");

    public static readonly Error InvalidTwoFactorCode =
        Error.Unauthorized("auth.invalid_two_factor_code", "The two-factor authentication code is invalid or has expired.");

    public static readonly Error InvalidRecoveryCode =
        Error.Unauthorized("auth.invalid_recovery_code", "The recovery code is invalid or has already been used.");
}
