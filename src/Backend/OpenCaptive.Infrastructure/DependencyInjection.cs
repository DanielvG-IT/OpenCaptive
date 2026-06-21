using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenCaptive.Application.Auth.Contracts;
using OpenCaptive.Application.Email;
using OpenCaptive.Application.Organizations.Contracts;
using OpenCaptive.Infrastructure.Email;
using OpenCaptive.Infrastructure.Identity;
using OpenCaptive.Infrastructure.Options;
using OpenCaptive.Infrastructure.Persistence;

namespace OpenCaptive.Infrastructure;

public static class DependencyInjection
{
  public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
  {
    services.Configure<JwtOptions>(configuration.GetSection("Authentication:Jwt"));
    services.Configure<RefreshTokenOptions>(configuration.GetSection("Authentication:RefreshToken"));

    services.AddIdentityCore<ApplicationUser>(options =>
    {
      options.User.RequireUniqueEmail = true;
    }).AddEntityFrameworkStores<OpenCaptiveDbContext>();

    services.AddDbContext<OpenCaptiveDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("Postgres")));

    services.AddScoped<IOrganizationService, Organizations.OrganizationService>();

    services.AddSingleton<IAccessTokenGenerator, JwtAccessTokenGenerator>();
    services.AddSingleton<ITokenHasher, Sha256TokenHasher>();

    services.AddScoped<IAuthService, AuthService>();
    services.AddScoped<ITransactionalEmailProvider, LoggingTransactionalEmailProvider>();

    return services;
  }
}
