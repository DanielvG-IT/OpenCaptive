using OpenCaptive.Application.Common;

namespace OpenCaptive.Application.Profile;

public interface IProfileService
{
  Task<Result<ProfileResponse>> GetProfileAsync(Guid userId, CancellationToken cancellationToken = default);

  Task<Result<TwoFactorSetupResponse>> SetupTwoFactorAsync(Guid userId, CancellationToken cancellationToken = default);
  Task<Result<TwoFactorEnableResponse>> EnableTwoFactorAsync(Guid userId, ChangeTwoFactorStateInput input, CancellationToken cancellationToken = default);
  Task<Result> DisableTwoFactorAsync(Guid userId, ChangeTwoFactorStateInput input, CancellationToken cancellationToken = default);
}