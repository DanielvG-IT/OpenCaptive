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
}
