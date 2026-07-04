using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenCaptive.Application.Email.Contracts;
using OpenCaptive.Application.Organizations.Contracts;
using OpenCaptive.Application.Profile;
using OpenCaptive.Application.Sites;
using OpenCaptive.Infrastructure.Auth;
using OpenCaptive.Infrastructure.Email;
using OpenCaptive.Infrastructure.Frontend;
using OpenCaptive.Infrastructure.Organizations;
using OpenCaptive.Infrastructure.Persistence;
using OpenCaptive.Infrastructure.Profile;
using OpenCaptive.Infrastructure.Sites;

namespace OpenCaptive.Infrastructure;

public static class DependencyInjection
{
  public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
  {
    services.AddOpenCaptiveAuthentication(configuration);

    services.AddOpenCaptiveEmail(configuration);

    services.Configure<FrontendOptions>(configuration.GetSection(FrontendOptions.SectionName));

    services.AddDbContext<OpenCaptiveDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("Postgres")));

    // Health checks for infrastructure-owned resources are registered here, beside the
    // resources they probe. Endpoint mapping (/health/live, /health/ready) stays in the API layer.
    services.AddHealthChecks()
        .AddDbContextCheck<OpenCaptiveDbContext>(name: "database_check", tags: ["ready"]);
    // Future infrastructure checks belong here too, e.g.:
    // .AddRedis(configuration.GetConnectionString("Redis")!, name: "redis_check", tags: ["ready"])
    // .AddUrlGroup(new Uri("https://sms-provider.com"), name: "sms_gateway_check", tags: ["ready"]);

    services.AddScoped<IOrganizationService, OrganizationService>();
    services.AddScoped<IProfileService, ProfileService>();
    services.AddScoped<ISiteService, SiteService>();

    services.AddScoped<IFrontendLinkFactory, FrontendLinkFactory>();

    return services;
  }
}