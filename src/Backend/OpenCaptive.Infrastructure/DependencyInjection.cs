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

    services.AddScoped<IOrganizationService, OrganizationService>();
    services.AddScoped<IProfileService, ProfileService>();
    services.AddScoped<ISiteService, SiteService>();

    services.AddScoped<IFrontendLinkFactory, FrontendLinkFactory>();

    return services;
  }
}