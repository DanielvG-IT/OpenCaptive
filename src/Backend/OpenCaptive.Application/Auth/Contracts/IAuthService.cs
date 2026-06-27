using OpenCaptive.Application.Auth.Models;
using OpenCaptive.Application.Common;

namespace OpenCaptive.Application.Auth.Contracts;

public interface IAuthService
{
  Task<Result<LoginResponse>> LoginAsync(LoginInput input, CancellationToken cancellationToken = default);
  Task<Result<RegisterResponse>> RegisterAsync(RegisterInput input, CancellationToken cancellationToken = default);
  Task<Result<TokenResponse>> RefreshAsync(RefreshInput input, CancellationToken cancellationToken = default);

  Task<Result<VerifyEmailReponse>> VerifyEmailAsync(VerifyEmailInput input, CancellationToken cancellationToken = default);
  Task<Result<TokenResponse>> VerifyTwoFactorAsync(VerifyMfaInput input, CancellationToken cancellationToken = default);
  Task<Result<RedeemRecoveryCodeResponse>> RedeemRecoveryCodeAsync(RedeemRecoveryCodeInput input, CancellationToken cancellationToken = default);
}
