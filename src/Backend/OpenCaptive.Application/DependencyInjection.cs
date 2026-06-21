using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using OpenCaptive.Application.Organizations.Validators;

namespace OpenCaptive.Application;

public static class DependencyInjection
{
  public static IServiceCollection AddApplication(this IServiceCollection services)
  {
    services.AddValidatorsFromAssemblyContaining<CreateOrganizationInputValidator>(ServiceLifetime.Singleton);

    return services;
  }
}
