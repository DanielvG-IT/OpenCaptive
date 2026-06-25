using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using OpenCaptive.Application.Auth.Contracts;
using OpenCaptive.Application.Email.Contracts;
using OpenCaptive.Application.Organizations.Contracts;

using OpenCaptive.Infrastructure.Auth;
using OpenCaptive.Infrastructure.Email;
using OpenCaptive.Infrastructure.Email.Rendering;
using OpenCaptive.Infrastructure.Frontend;
using OpenCaptive.Infrastructure.Persistence;
using RazorLight;

namespace OpenCaptive.Infrastructure;

public static class DependencyInjection
{
  public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
  {
    services.Configure<JwtOptions>(configuration.GetSection("Authentication:Jwt"));
    services.Configure<RefreshTokenOptions>(configuration.GetSection("Authentication:RefreshToken"));
    services.Configure<PasswordResetOptions>(configuration.GetSection("Authentication:PasswordReset"));
    services.Configure<EmailVerificationOptions>(configuration.GetSection("Authentication:EmailVerification"));
    services.Configure<TwoFactorAuthenticationOptions>(configuration.GetSection("Authentication:TwoFactorAuthentication"));

    services.Configure<EmailOptions>(configuration.GetSection("Email"));
    services.Configure<FrontendOptions>(configuration.GetSection("Frontend"));

    services.AddOptions<EmailVerificationTokenProviderOptions>()
      .Configure<IOptions<EmailVerificationOptions>>((options, emailVerificationOptions) =>
      {
        options.Name = "EmailVerification";
        options.TokenLifespan = emailVerificationOptions.Value.TokenLifetime;
      });

    services.AddOptions<PasswordResetTokenProviderOptions>()
      .Configure<IOptions<PasswordResetOptions>>((options, passwordResetOptions) =>
      {
        options.Name = "PasswordReset";
        options.TokenLifespan = passwordResetOptions.Value.TokenLifetime;
      });

    services.AddIdentityCore<ApplicationUser>(options =>
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

    services.AddOptions<IdentityOptions>()
      .Configure<IOptions<TwoFactorAuthenticationOptions>>((identityOptions, twoFactorAuthenticationOptions) =>
      {
        identityOptions.Tokens.AuthenticatorIssuer = twoFactorAuthenticationOptions.Value.AuthenticatorIssuer;
      });

    services.AddDbContext<OpenCaptiveDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("Postgres")));

    services.AddScoped<IOrganizationService, Organizations.OrganizationService>();
    services.AddScoped<IAuthService, AuthService>();

    services.AddSingleton<IAccessTokenGenerator, JwtAccessTokenGenerator>();
    services.AddSingleton<ITokenHasher, Sha256TokenHasher>();
    services.AddSingleton<IRazorLightEngine>(_ =>
    {
      var infrastructureAssembly = typeof(RazorEmailTemplateRenderer).Assembly;
      return new RazorLightEngineBuilder()
        .UseEmbeddedResourcesProject(infrastructureAssembly, "OpenCaptive.Infrastructure.Email.Rendering.Templates")
        .UseMemoryCachingProvider()
        .Build();
    });

    services.AddScoped<ITransactionalEmailProvider, SmtpTransactionalEmailProvider>();
    services.AddScoped<IEmailTemplateRenderer, RazorEmailTemplateRenderer>();
    services.AddScoped<IFrontendLinkFactory, FrontendLinkFactory>();

    return services;
  }
}
