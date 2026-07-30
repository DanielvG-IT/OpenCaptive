using OpenCaptive.Application.Common;

namespace OpenCaptive.Application.Profile;

public static class ProfileErrors
{
    public static readonly Error TwoFactorAlreadyEnabled =
        Error.Conflict("profile.two_factor_already_enabled", "Two-factor authentication is already enabled on this account.");

    public static readonly Error TwoFactorNotEnabled =
        Error.Conflict("profile.two_factor_not_enabled", "Two-factor authentication is not enabled on this account.");

    public static readonly Error InvalidTwoFactorCode =
        Error.Unauthorized("profile.invalid_two_factor_code", "The two-factor authentication code is invalid or has expired.");

    public static readonly Error TwoFactorSetupFailed =
        Error.Failure("profile.setup_failed", "Failed to set up two-factor authentication.");

    public static readonly Error TwoFactorUpdateFailed =
        Error.Failure("profile.two_factor_update_failed", "Failed to update two-factor authentication settings.");
}