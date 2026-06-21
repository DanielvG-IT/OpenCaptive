using OpenCaptive.Application.Auth.Models;
using OpenCaptive.Application.Common;

namespace OpenCaptive.Application.Auth.Contracts;

public interface IAuthService
{
  Task<Result<LoginResponse>> LoginAsync(LoginInput input, CancellationToken cancellationToken = default);
  Task<Result<AuthResponse>> RefreshAsync(RefreshInput input, CancellationToken cancellationToken = default);
  Task<Result<MeResponse>> MeAsync(Guid userId, CancellationToken cancellationToken = default);
  // Task<Result<LoginResponse>> VerifyMfaAsync(VerifyMfaInput input, CancellationToken cancellationToken = default);
  // Task<Result<AuthResponse>> RegisterAsync(RegisterInput input, CancellationToken cancellationToken = default);
  // Task<Result<LoginResponse>> VerifyEmailAsync(VerifyEmailInput input, CancellationToken cancellationToken = default);
}
