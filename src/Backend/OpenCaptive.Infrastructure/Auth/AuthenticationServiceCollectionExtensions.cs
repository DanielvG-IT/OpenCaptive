using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OpenCaptive.Application.Auth.Contracts;
using OpenCaptive.Infrastructure.Persistence;


namespace OpenCaptive.Infrastructure.Auth;

public static class AuthenticationServiceCollectionExtensions
{
  public static IServiceCollection AddOpenCaptiveAuthentication(this IServiceCollection services, IConfiguration configuration)
  {
    services
        .AddOptions<JwtOptions>()
        .Bind(configuration.GetSection(JwtOptions.SectionName))
        .ValidateDataAnnotations()
        .ValidateOnStart();

    services
        .AddOptions<RefreshTokenOptions>()
        .Bind(configuration.GetSection(RefreshTokenOptions.SectionName))
        .ValidateDataAnnotations()
        .ValidateOnStart();

    services
        .AddOptions<PasswordResetOptions>()
        .Bind(configuration.GetSection(PasswordResetOptions.SectionName))
        .ValidateDataAnnotations()
        .ValidateOnStart();

    services
        .AddOptions<EmailVerificationOptions>()
        .Bind(configuration.GetSection(EmailVerificationOptions.SectionName))
        .ValidateDataAnnotations()
        .ValidateOnStart();

    services
        .AddOptions<TwoFactorAuthenticationOptions>()
        .Bind(configuration.GetSection(TwoFactorAuthenticationOptions.SectionName))
        .ValidateDataAnnotations()
        .ValidateOnStart();

    services
        .AddOptions<IdentityOptions>()
        .Configure<IOptions<TwoFactorAuthenticationOptions>>((identity, options) =>
            {
              identity.Tokens.AuthenticatorIssuer = options.Value.AuthenticatorIssuer;
            });

    services
        .AddOptions<EmailVerificationTokenProviderOptions>()
        .Configure<IOptions<EmailVerificationOptions>>((provider, options) =>
            {
              provider.Name = "EmailVerification";
              provider.TokenLifespan = options.Value.TokenLifetime;
            });

    services
        .AddOptions<PasswordResetTokenProviderOptions>()
        .Configure<IOptions<PasswordResetOptions>>((provider, options) =>
            {
              provider.Name = "PasswordReset";
              provider.TokenLifespan = options.Value.TokenLifetime;
            });

    services
        .AddIdentityCore<ApplicationUser>(options =>
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

          options.Tokens.EmailConfirmationTokenProvider = "EmailVerification";
          options.Tokens.PasswordResetTokenProvider = "PasswordReset";
        })
        .AddEntityFrameworkStores<OpenCaptiveDbContext>()
        .AddSignInManager()
        .AddDefaultTokenProviders()
        .AddTokenProvider<EmailVerificationTokenProvider<ApplicationUser>>("EmailVerification")
        .AddTokenProvider<PasswordResetTokenProvider<ApplicationUser>>("PasswordReset");

    services.AddScoped<IAuthService, AuthService>();

    services.AddSingleton<IAccessTokenGenerator, JwtAccessTokenGenerator>();
    services.AddSingleton<ITokenHasher, Sha256TokenHasher>();

    return services;
  }
}