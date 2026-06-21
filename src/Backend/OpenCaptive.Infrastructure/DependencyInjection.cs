using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenCaptive.Application.Auth;
using OpenCaptive.Application.Organizations;
using OpenCaptive.Infrastructure.Identity;
using OpenCaptive.Infrastructure.Options;
using OpenCaptive.Infrastructure.Persistence;
using OpenCaptive.Infrastructure.Repositories;

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
    services.AddScoped<IOrganizationRepository, OrganizationRepository>();

    services.AddSingleton<IAccessTokenGenerator, JwtAccessTokenGenerator>();
    services.AddSingleton<ITokenHasher, Sha256TokenHasher>();

    services.AddScoped<IAuthService, AuthService>();

    return services;
  }
}
