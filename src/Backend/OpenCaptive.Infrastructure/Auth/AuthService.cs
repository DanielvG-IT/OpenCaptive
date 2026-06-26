using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;
using OpenCaptive.Application.Auth.Contracts;
using OpenCaptive.Application.Auth.Errors;
using OpenCaptive.Application.Auth.Models;
using OpenCaptive.Application.Common;
using OpenCaptive.Application.Email.Contracts;
using OpenCaptive.Application.Email.Models;
using OpenCaptive.Application.Organizations.Errors;
using OpenCaptive.Domain.Auth;
using OpenCaptive.Domain.Organizations;
using OpenCaptive.Infrastructure.Persistence;

namespace OpenCaptive.Infrastructure.Auth;

public sealed class AuthService(
    ITokenHasher tokenHasher,
    ILogger<AuthService> logger,
    OpenCaptiveDbContext dbContext,
    IFrontendLinkFactory frontendLinkFactory,
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    ITransactionalEmailProvider emailProvider,
    IAccessTokenGenerator accessTokenGenerator,
    IEmailTemplateRenderer emailTemplateRenderer,
    IOptions<RefreshTokenOptions> refreshTokenOptions,
    IOptions<EmailVerificationOptions> emailVerificationOptions
    ) : IAuthService
{
  private readonly ILogger<AuthService> _logger = logger;
  private readonly ITokenHasher _tokenHasher = tokenHasher;
  private readonly OpenCaptiveDbContext _dbContext = dbContext;
  private readonly UserManager<ApplicationUser> _userManager = userManager;
  private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
  private readonly ITransactionalEmailProvider _emailProvider = emailProvider;
  private readonly IAccessTokenGenerator _accessTokenGenerator = accessTokenGenerator;
  private readonly IEmailTemplateRenderer _emailTemplateRenderer = emailTemplateRenderer;
  private readonly IFrontendLinkFactory _frontendLinkFactory = frontendLinkFactory;

  // TODO(security, deferred): No absolute session lifetime — every refresh resets this 30-day
  // window, so an actively-refreshed session never expires. Decide whether to cap total session
  // age (would need an "issued/family-created at" timestamp that survives rotation).
  private readonly RefreshTokenOptions _refreshTokenOptions = refreshTokenOptions.Value;
  private readonly EmailVerificationOptions _emailVerificationOptions = emailVerificationOptions.Value;

  public async Task<Result<LoginResponse>> LoginAsync(LoginInput input, CancellationToken cancellationToken = default)
  {
    var user = await _userManager.FindByEmailAsync(input.Email);

    if (user is null || !user.IsActive)
    {
      // TODO: Timing oracle mitigation: Always compute a hash to equalize response times
      return Result.Failure<LoginResponse>(AuthErrors.InvalidCredentials);
    }

    var signInResult = await _signInManager.CheckPasswordSignInAsync(user, input.Password, lockoutOnFailure: true);

    if (signInResult.IsLockedOut)
    {
      return Result.Success(new LoginResponse(LoginStatus.AccountLocked, null, null));
    }

    if (signInResult.IsNotAllowed && !await _userManager.IsEmailConfirmedAsync(user))
    {
      return Result.Success(new LoginResponse(LoginStatus.EmailVerificationRequired, null, null));
    }

    if (!signInResult.Succeeded)
    {
      return Result.Failure<LoginResponse>(AuthErrors.InvalidCredentials);
    }

    if (await _userManager.GetTwoFactorEnabledAsync(user))
    {
      return Result.Success(new LoginResponse(LoginStatus.MfaRequired, null, "challengeCode"));
    }

    var tokens = await GenerateTokensAsync(user, Guid.CreateVersion7(), cancellationToken);
    await MarkUserAuthenticatedAsync(user);

    return Result.Success(
        new LoginResponse(
            LoginStatus.Success,
            new TokenResponse(tokens.AccessToken, tokens.AccessTokenExpiresAt, tokens.RefreshToken, tokens.RefreshTokenExpiresAt),
            null));
  }

  public async Task<Result<RegisterResponse>> RegisterAsync(RegisterInput input, CancellationToken cancellationToken = default)
  {
    var existingUser = await _userManager.FindByEmailAsync(input.Email);
    if (existingUser is not null)
    {
      return Result.Failure<RegisterResponse>(AuthErrors.UserAlreadyExists);
    }

    try
    {
      await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

      // Creating Organization
      var newOrg = Organization.Create(input.OrganizationName, input.OrganizationSlug);
      _dbContext.Organizations.Add(newOrg);
      await _dbContext.SaveChangesAsync(cancellationToken);

      // Creating User
      var newUser = new ApplicationUser
      {
        FirstName = input.FirstName,
        LastName = input.LastName,
        UserName = input.Email,
        Email = input.Email
      };
      var createUserResult = await _userManager.CreateAsync(newUser, input.Password);
      if (!createUserResult.Succeeded)
      {
        return Result.Failure<RegisterResponse>(AuthErrors.UserCreationFailed);
      }

      // Creating Org<->User relationship
      var newMembership = OrganizationMembership.Create(newUser.Id, newOrg.Id, OrganizationRole.Owner);
      _dbContext.OrganizationMemberships.Add(newMembership);
      await _dbContext.SaveChangesAsync(cancellationToken);

      await transaction.CommitAsync(cancellationToken);

      var isVerificationEmailSent = await SendVerificationEmail(newUser, cancellationToken);
      return Result.Success(new RegisterResponse(isVerificationEmailSent));
    }
    catch (DbUpdateException ex) when (ex.InnerException is PostgresException pg && pg.SqlState == PostgresErrorCodes.UniqueViolation)
    {
      return Result.Failure<RegisterResponse>(OrganizationErrors.SlugAlreadyExists(input.OrganizationSlug));
    }
  }

  public async Task<Result<TokenResponse>> RefreshAsync(RefreshInput input, CancellationToken cancellationToken = default)
  {
    var refreshTokenHash = _tokenHasher.Hash(input.RefreshToken);
    var now = DateTimeOffset.UtcNow;

    var refreshToken = await _dbContext.RefreshTokens.SingleOrDefaultAsync(x => x.TokenHash == refreshTokenHash, cancellationToken);
    if (refreshToken is null || refreshToken.IsExpired)
    {
      return Result.Failure<TokenResponse>(AuthErrors.InvalidRefreshToken);
    }

    // Family revocation detection
    if (refreshToken.IsRevoked)
    {
      await _dbContext.RefreshTokens
          .Where(x => x.UserId == refreshToken.UserId && x.FamilyId == refreshToken.FamilyId && x.RevokedAt == null)
          .ExecuteUpdateAsync(s => s.SetProperty(x => x.RevokedAt, now), cancellationToken);

      return Result.Failure<TokenResponse>(AuthErrors.InvalidRefreshToken);
    }

    var user = await _userManager.FindByIdAsync(refreshToken.UserId.ToString());
    if (user is null || string.IsNullOrWhiteSpace(user.Email) || !user.IsActive)
    {
      return Result.Failure<TokenResponse>(AuthErrors.InvalidRefreshToken);
    }

    var securityStamp = await _userManager.GetSecurityStampAsync(user);
    if (!string.Equals(refreshToken.SecurityStamp, securityStamp, StringComparison.Ordinal))
    {
      return Result.Failure<TokenResponse>(AuthErrors.InvalidRefreshToken);
    }

    var rowsAffected = await _dbContext.RefreshTokens
        .Where(x => x.Id == refreshToken.Id && x.RevokedAt == null)
        .ExecuteUpdateAsync(s => s.SetProperty(x => x.RevokedAt, now), cancellationToken);

    if (rowsAffected == 0)
    {
      await _dbContext.RefreshTokens
          .Where(x => x.UserId == refreshToken.UserId && x.FamilyId == refreshToken.FamilyId && x.RevokedAt == null)
          .ExecuteUpdateAsync(s => s.SetProperty(x => x.RevokedAt, now), cancellationToken);

      return Result.Failure<TokenResponse>(AuthErrors.InvalidRefreshToken);
    }

    var tokens = await GenerateTokensAsync(user, refreshToken.FamilyId, cancellationToken);

    return Result.Success(new TokenResponse(tokens.AccessToken, tokens.AccessTokenExpiresAt, tokens.RefreshToken, tokens.RefreshTokenExpiresAt));
  }

  public async Task<Result<MeResponse>> MeAsync(Guid userId, CancellationToken cancellationToken = default)
  {
    var user = await _userManager.FindByIdAsync(userId.ToString());
    if (user is null || string.IsNullOrWhiteSpace(user.Email) || !user.IsActive)
    {
      return Result.Failure<MeResponse>(AuthErrors.UserNotFound);
    }

    return Result.Success(new MeResponse(user.Id, user.Email, user.FirstName, user.LastName, user.EmailConfirmed, user.TwoFactorEnabled));
  }

  public Task<Result<TokenResponse>> VerifyMfaAsync(VerifyMfaInput input, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }


  // ===== Helpers ======
  private async Task<bool> SendVerificationEmail(ApplicationUser user, CancellationToken cancellationToken)
  {
    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
    var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
    var verificationUrl = _frontendLinkFactory.CreateVerifyEmailLink(user.Id, encodedToken);

    try
    {
      var bodies = await _emailTemplateRenderer.RenderAsync(
          EmailTemplate.VerifyEmail,
          new VerifyEmailTemplateModel(
            user.FirstName,
            verificationUrl,
            _emailVerificationOptions.TokenLifetime
          )
      );

      var email = new TransactionalEmail(
          ToAddress: user.Email!,
          ToName: user.FirstName,
          Subject: "OpenCaptive email verification",
          Bodies: bodies
      );

      await _emailProvider.SendAsync(email, cancellationToken);
      return true;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to send verification email to {Email}", user.Email);
      return false;
    }
  }

  private async Task MarkUserAuthenticatedAsync(ApplicationUser user)
  {
    var lastLoginAt = DateTimeOffset.UtcNow;

    await _dbContext.Users
      .Where(x => x.Id == user.Id)
      .ExecuteUpdateAsync(s => s.SetProperty(x => x.LastLoginAt, lastLoginAt));
  }

  private static string GenerateRefreshTokenValue()
  {
    Span<byte> bytes = stackalloc byte[32];
    RandomNumberGenerator.Fill(bytes);
    return WebEncoders.Base64UrlEncode(bytes);
  }

  private async Task<TokenResponse> GenerateTokensAsync(ApplicationUser user, Guid familyId, CancellationToken cancellationToken)
  {
    var now = DateTimeOffset.UtcNow;

    var membership = await _dbContext.OrganizationMemberships.SingleOrDefaultAsync(x => x.UserId == user.Id, cancellationToken);

    var accessToken = _accessTokenGenerator.Generate(new AccessTokenRequest(user.Id, user.Email!, membership?.OrganizationId, membership?.Role));

    var refreshTokenExpiresAt = now.Add(_refreshTokenOptions.Lifetime);
    var rawRefreshToken = GenerateRefreshTokenValue();
    var securityStamp = await _userManager.GetSecurityStampAsync(user);

    _dbContext.RefreshTokens.Add(RefreshToken.Create(user.Id, _tokenHasher.Hash(rawRefreshToken), securityStamp, refreshTokenExpiresAt, familyId));
    await _dbContext.SaveChangesAsync(cancellationToken);

    return new TokenResponse(
        accessToken.Token,
        accessToken.ExpiresAt,
        rawRefreshToken,
        refreshTokenExpiresAt);
  }
}
