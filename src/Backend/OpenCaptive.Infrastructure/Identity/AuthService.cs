using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OpenCaptive.Application.Auth.Contracts;
using OpenCaptive.Application.Auth.Errors;
using OpenCaptive.Application.Auth.Models;
using OpenCaptive.Application.Common;
using OpenCaptive.Domain.Auth;
using OpenCaptive.Infrastructure.Options;
using OpenCaptive.Infrastructure.Persistence;

namespace OpenCaptive.Infrastructure.Identity;

public sealed class AuthService(
    ITokenHasher tokenHasher,
    OpenCaptiveDbContext dbContext,
    UserManager<ApplicationUser> userManager,
    IAccessTokenGenerator accessTokenGenerator,
    IOptions<RefreshTokenOptions> refreshTokenOptions
) : IAuthService
{
  private readonly ITokenHasher _tokenHasher = tokenHasher;
  private readonly OpenCaptiveDbContext _dbContext = dbContext;
  private readonly UserManager<ApplicationUser> _userManager = userManager;
  private readonly IAccessTokenGenerator _accessTokenGenerator = accessTokenGenerator;

  // TODO(security, deferred): No absolute session lifetime — every refresh resets this 30-day
  // window, so an actively-refreshed session never expires. Decide whether to cap total session
  // age (would need an "issued/family-created at" timestamp that survives rotation).
  private readonly TimeSpan RefreshTokenLifetime = refreshTokenOptions.Value.Lifetime;

  public async Task<Result<LoginResponse>> LoginAsync(LoginInput input, CancellationToken cancellationToken = default)
  {
    if (string.IsNullOrWhiteSpace(input.Email) || string.IsNullOrWhiteSpace(input.Password))
    {
      return Result.Failure<LoginResponse>(AuthErrors.InvalidCredentials);
    }

    // TODO(security, deferred): No brute-force lockout. UserManager.CheckPasswordAsync does not
    // track failed attempts — that's SignInManager/CheckPasswordSignInAsync behavior. Nothing
    // currently throttles password guessing against a known email. Wire up Identity lockout
    // (AccessFailedAsync + IsLockedOutAsync, or switch to CheckPasswordSignInAsync) and/or
    // add rate limiting on /auth/login.
    // TODO(security, deferred): User-enumeration timing oracle. The null-user path skips
    // password hashing, so real accounts respond measurably slower than fake ones. Mitigate by
    // always running PBKDF2 — verify input.Password against a precomputed dummy hash from
    // _userManager.PasswordHasher (a new ApplicationUser with null PasswordHash will NOT do this).
    var user = await _userManager.FindByEmailAsync(input.Email);
    if (user is null || !user.IsActive)
    {
      return Result.Failure<LoginResponse>(AuthErrors.InvalidCredentials);
    }

    if (!await _userManager.CheckPasswordAsync(user, input.Password))
    {
      return Result.Failure<LoginResponse>(AuthErrors.InvalidCredentials);
    }

    if (!await _userManager.IsEmailConfirmedAsync(user))
    {
      // TODO: Email verification flow
      return Result.Success(new LoginResponse(LoginStatus.EmailVerificationRequired, null, null));
    }

    if (await _userManager.GetTwoFactorEnabledAsync(user))
    {
      // TODO: MFA challenge flow
      return Result.Success(new LoginResponse(LoginStatus.MfaRequired, null, "challengeCode"));
    }

    var tokens = await GenerateTokensAsync(user, Guid.CreateVersion7(), cancellationToken);

    await MarkUserAuthenticatedAsync(user);

    return Result.Success(
        new LoginResponse(
            LoginStatus.Success,
            new AuthResponse(
                tokens.AccessToken,
                tokens.AccessTokenExpiresAt,
                tokens.RefreshToken,
                tokens.RefreshTokenExpiresAt),
            null));
  }

  public async Task<Result<AuthResponse>> RefreshAsync(RefreshInput input, CancellationToken cancellationToken = default)
  {
    var refreshTokenHash = _tokenHasher.Hash(input.RefreshToken);

    // TODO(security, deferred): Concurrency race. Two parallel refreshes with the same token can
    // both pass IsActive before either revokes, yielding two valid token chains from one token.
    // Needs a conditional/atomic revoke (concurrency token or UPDATE ... WHERE RevokedAt IS NULL).
    var refreshToken = await _dbContext.RefreshTokens.SingleOrDefaultAsync(x => x.TokenHash == refreshTokenHash, cancellationToken);
    if (refreshToken is null || refreshToken.IsExpired)
    {
      return Result.Failure<AuthResponse>(AuthErrors.InvalidRefreshToken);
    }

    // Refresh token reuse detection (OAuth BCP). 
    // Revoke the entire token family and force re-auth, rather than failing silently here.
    if (refreshToken.IsRevoked)
    {
      var now = DateTimeOffset.UtcNow;
      await _dbContext.RefreshTokens
        .Where(x => x.UserId == refreshToken.UserId && x.FamilyId == refreshToken.FamilyId && x.RevokedAt == null)
        .ExecuteUpdateAsync(s => s.SetProperty(x => x.RevokedAt, now), cancellationToken);

      return Result.Failure<AuthResponse>(AuthErrors.InvalidRefreshToken);
    }

    var user = await _userManager.FindByIdAsync(refreshToken.UserId.ToString());
    if (user is null || string.IsNullOrWhiteSpace(user.Email) || !user.IsActive)
    {
      return Result.Failure<AuthResponse>(AuthErrors.InvalidRefreshToken);
    }

    refreshToken.Revoke();
    _dbContext.RefreshTokens.Update(refreshToken);

    var tokens = await GenerateTokensAsync(user, refreshToken.FamilyId, cancellationToken);

    return Result.Success(
        new AuthResponse(
            tokens.AccessToken,
            tokens.AccessTokenExpiresAt,
            tokens.RefreshToken,
            tokens.RefreshTokenExpiresAt));
  }

  public async Task<Result<MeResponse>> MeAsync(Guid userId, CancellationToken cancellationToken = default)
  {
    var user = await _userManager.FindByIdAsync(userId.ToString());
    if (user is null || string.IsNullOrWhiteSpace(user.Email) || !user.IsActive)
    {
      return Result.Failure<MeResponse>(AuthErrors.UserNotFound);
    }

    return Result.Success(new MeResponse(user.Id, user.Email, user.EmailConfirmed, user.TwoFactorEnabled));
  }

  // public Task<Result<LoginResponse>> VerifyMfaAsync(VerifyMfaInput input, CancellationToken cancellationToken = default)
  // {
  //   throw new NotImplementedException();
  // }

  // public Task<Result<LoginResponse>> VerifyEmailAsync(VerifyEmailInput input, CancellationToken cancellationToken = default)
  // {
  //   throw new NotImplementedException();
  // }

  // public async Task<Result<AuthResponse>> RegisterAsync(RegisterInput input, CancellationToken cancellationToken = default)
  // {
  //   throw new NotImplementedException();
  // }



  // ===== Helpers ======
  private async Task MarkUserAuthenticatedAsync(ApplicationUser user)
  {
    user.LastLoginAt = DateTimeOffset.UtcNow;
    await _userManager.UpdateAsync(user);
  }

  private static string GenerateRefreshTokenValue()
  {
    Span<byte> bytes = stackalloc byte[32];
    RandomNumberGenerator.Fill(bytes);
    return WebEncoders.Base64UrlEncode(bytes);
  }

  private async Task<TokenPair> GenerateTokensAsync(ApplicationUser user, Guid familyId, CancellationToken cancellationToken)
  {
    var now = DateTimeOffset.UtcNow;

    // Single-org for now: relax to per-request org selection when multi-org lands.
    var membership = await _dbContext.OrganizationMemberships.SingleOrDefaultAsync(x => x.UserId == user.Id, cancellationToken);

    var accessToken = _accessTokenGenerator.Generate(new AccessTokenRequest(user.Id, user.Email!, membership?.OrganizationId, membership?.Role));

    var refreshTokenExpiresAt = now.Add(RefreshTokenLifetime);
    var rawRefreshToken = GenerateRefreshTokenValue();

    _dbContext.RefreshTokens.Add(RefreshToken.Create(user.Id, _tokenHasher.Hash(rawRefreshToken), refreshTokenExpiresAt, familyId));

    await _dbContext.SaveChangesAsync(cancellationToken);

    return new TokenPair(
        accessToken.Token,
        accessToken.ExpiresAt,
        rawRefreshToken,
        refreshTokenExpiresAt);
  }

  private sealed record TokenPair(
      string AccessToken,
      DateTimeOffset AccessTokenExpiresAt,
      string RefreshToken,
      DateTimeOffset RefreshTokenExpiresAt
  );
}