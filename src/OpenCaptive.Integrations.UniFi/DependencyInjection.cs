using Microsoft.Extensions.DependencyInjection;

namespace OpenCaptive.Integrations.UniFi;

public static class DependencyInjection
{
  public static IServiceCollection AddUniFiIntegration(this IServiceCollection services)
  {
    return services;
  }
}