using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenCaptive.Application.Organizations;
using OpenCaptive.Infrastructure.Persistence;
using OpenCaptive.Infrastructure.Repositories;

namespace OpenCaptive.Infrastructure;

public static class DependencyInjection
{
  public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
  {
    services.AddDbContext<OpenCaptiveDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("Postgres")));
    services.AddScoped<IOrganizationRepository, OrganizationRepository>();

    return services;
  }
}
