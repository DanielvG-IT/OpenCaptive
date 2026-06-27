using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using OpenCaptive.Application.Auth.Errors;
using OpenCaptive.Application.Common;
using OpenCaptive.Application.Profile;
using OpenCaptive.Infrastructure.Auth;

namespace OpenCaptive.Infrastructure.Profile;

public sealed class ProfileService(UserManager<ApplicationUser> userManager, IOptions<TwoFactorAuthenticationOptions> twoFactorOptions) : IProfileService
{
  private readonly UserManager<ApplicationUser> _userManager = userManager;
  private readonly TwoFactorAuthenticationOptions _twoFactorOptions = twoFactorOptions.Value;

  public async Task<Result<ProfileResponse>> GetProfileAsync(Guid userId, CancellationToken cancellationToken = default)
  {
    var user = await _userManager.FindByIdAsync(userId.ToString());
    if (user is null || string.IsNullOrWhiteSpace(user.Email) || !user.IsActive)
    {
      return Result.Failure<ProfileResponse>(AuthErrors.UserNotFound);
    }

    return Result.Success(new ProfileResponse(user.Id, user.Email, user.FirstName, user.LastName, user.EmailConfirmed, user.TwoFactorEnabled, user.LastLoginAt));
  }

  public async Task<Result<TwoFactorSetupResponse>> SetupTwoFactorAsync(Guid userId, CancellationToken cancellationToken = default)
  {
    var user = await _userManager.FindByIdAsync(userId.ToString());
    if (user is null || string.IsNullOrWhiteSpace(user.Email) || !user.IsActive)
    {
      return Result.Failure<TwoFactorSetupResponse>(AuthErrors.UserNotFound);
    }

    if (user.TwoFactorEnabled)
    {
      return Result.Failure<TwoFactorSetupResponse>(ProfileErrors.TwoFactorAlreadyEnabled);
    }

    var resetResult = await _userManager.ResetAuthenticatorKeyAsync(user);
    if (!resetResult.Succeeded)
    {
      return Result.Failure<TwoFactorSetupResponse>(ProfileErrors.TwoFactorSetupFailed);
    }

    var rawKey = await _userManager.GetAuthenticatorKeyAsync(user);
    if (string.IsNullOrWhiteSpace(rawKey))
    {
      return Result.Failure<TwoFactorSetupResponse>(ProfileErrors.TwoFactorSetupFailed);
    }

    string escapedIssuer = Uri.EscapeDataString(_twoFactorOptions.AuthenticatorIssuer);
    string escapedEmail = Uri.EscapeDataString(user.Email);
    string otpAuthUri = $"otpauth://totp/{escapedIssuer}:{escapedEmail}?secret={rawKey}&issuer={escapedIssuer}";

    return Result.Success(new TwoFactorSetupResponse(otpAuthUri, rawKey));
  }

  public async Task<Result> EnableTwoFactorAsync(Guid userId, ChangeTwoFactorStateInput input, CancellationToken cancellationToken = default)
  {
    var user = await _userManager.FindByIdAsync(userId.ToString());
    if (user is null || string.IsNullOrWhiteSpace(user.Email) || !user.IsActive)
    {
      return Result.Failure(AuthErrors.UserNotFound);
    }

    if (user.TwoFactorEnabled)
    {
      return Result.Failure(ProfileErrors.TwoFactorAlreadyEnabled);
    }

    if (!await _userManager.VerifyTwoFactorTokenAsync(user, TokenOptions.DefaultAuthenticatorProvider, input.Code))
    {
      return Result.Failure(ProfileErrors.InvalidTwoFactorCode);
    }

    var result = await _userManager.SetTwoFactorEnabledAsync(user, true);
    if (!result.Succeeded)
    {
      return Result.Failure(ProfileErrors.TwoFactorUpdateFailed);
    }

    return Result.Success();
  }

  public async Task<Result> DisableTwoFactorAsync(Guid userId, ChangeTwoFactorStateInput input, CancellationToken cancellationToken = default)
  {
    var user = await _userManager.FindByIdAsync(userId.ToString());
    if (user is null || string.IsNullOrWhiteSpace(user.Email) || !user.IsActive)
    {
      return Result.Failure(AuthErrors.UserNotFound);
    }

    if (!user.TwoFactorEnabled)
    {
      return Result.Failure(ProfileErrors.TwoFactorNotEnabled);
    }

    if (!await _userManager.VerifyTwoFactorTokenAsync(user, TokenOptions.DefaultAuthenticatorProvider, input.Code))
    {
      return Result.Failure(ProfileErrors.InvalidTwoFactorCode);
    }

    var disableResult = await _userManager.SetTwoFactorEnabledAsync(user, false);
    if (!disableResult.Succeeded)
    {
      return Result.Failure(ProfileErrors.TwoFactorUpdateFailed);
    }

    // If stamp update fails, existing sessions stay valid!
    // Acceptable tradeoff over rolling back the disable.
    await _userManager.UpdateSecurityStampAsync(user);

    return Result.Success();
  }
}