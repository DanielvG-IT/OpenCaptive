using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using OpenCaptive.Application.Organizations;

namespace OpenCaptive.Application;

public static class DependencyInjection
{
  public static IServiceCollection AddApplication(this IServiceCollection services)
  {
    services.AddValidatorsFromAssemblyContaining<IOrganizationService>(ServiceLifetime.Singleton);

    services.AddScoped<IOrganizationService, OrganizationService>();

    return services;
  }
}
