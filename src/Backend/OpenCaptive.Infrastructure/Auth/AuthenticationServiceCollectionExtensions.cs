using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OpenCaptive.Application.Auth.Contracts;
using OpenCaptive.Infrastructure.Common.Options;
using OpenCaptive.Infrastructure.Persistence;

namespace OpenCaptive.Infrastructure.Auth;

public static class AuthenticationServiceCollectionExtensions
{
  public static IServiceCollection AddOpenCaptiveAuthentication(this IServiceCollection services, IConfiguration configuration)
  {
    services.AddValidatedOptions<JwtOptions>(configuration);
    services.AddValidatedOptions<RefreshTokenOptions>(configuration);
    services.AddValidatedOptions<PasswordResetOptions>(configuration);
    services.AddValidatedOptions<EmailVerificationOptions>(configuration);
    services.AddValidatedOptions<TwoFactorAuthenticationOptions>(configuration);

    ConfigureTwoFactorAuthentication(services);
    ConfigureEmailVerificationProvider(services);
    ConfigurePasswordResetProvider(services);

    services
        .AddIdentityCore<ApplicationUser>(ConfigureIdentity)
        .AddEntityFrameworkStores<OpenCaptiveDbContext>()
        .AddSignInManager()
        .AddDefaultTokenProviders()
        .AddTokenProvider<EmailVerificationTokenProvider<ApplicationUser>>(TokenProviders.EmailVerification)
        .AddTokenProvider<PasswordResetTokenProvider<ApplicationUser>>(TokenProviders.PasswordReset);

    services.AddScoped<IAuthService, AuthService>();

    services.AddSingleton<IAccessTokenGenerator, JwtAccessTokenGenerator>();
    services.AddSingleton<ITokenHasher, Sha256TokenHasher>();

    return services;
  }

  private static void ConfigureIdentity(IdentityOptions options)
  {
    options.User.RequireUniqueEmail = true;

    options.Password.RequiredLength = 12;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;

    options.Lockout.AllowedForNewUsers = true;
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);

    options.SignIn.RequireConfirmedEmail = true;

    options.Tokens.EmailConfirmationTokenProvider = TokenProviders.EmailVerification;
    options.Tokens.PasswordResetTokenProvider = TokenProviders.PasswordReset;
  }

  private static void ConfigureTwoFactorAuthentication(IServiceCollection services)
  {
    services
        .AddOptions<IdentityOptions>()
        .Configure<IOptions<TwoFactorAuthenticationOptions>>((identity, options) =>
            {
              identity.Tokens.AuthenticatorIssuer =
                  options.Value.AuthenticatorIssuer;
            });
  }

  private static void ConfigureEmailVerificationProvider(IServiceCollection services)
  {
    services
        .AddOptions<EmailVerificationTokenProviderOptions>()
        .Configure<IOptions<EmailVerificationOptions>>((provider, options) =>
            {
              provider.Name = TokenProviders.EmailVerification;
              provider.TokenLifespan = options.Value.TokenLifetime;
            });
  }

  private static void ConfigurePasswordResetProvider(IServiceCollection services)
  {
    services
        .AddOptions<PasswordResetTokenProviderOptions>()
        .Configure<IOptions<PasswordResetOptions>>((provider, options) =>
            {
              provider.Name = TokenProviders.PasswordReset;
              provider.TokenLifespan = options.Value.TokenLifetime;
            });
  }
}

public static class TokenProviders
{
  public const string EmailVerification = nameof(EmailVerification);
  public const string PasswordReset = nameof(PasswordReset);
}