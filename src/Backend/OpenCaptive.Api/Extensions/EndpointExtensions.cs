using System.Reflection;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using OpenCaptive.Api.Endpoints;

namespace OpenCaptive.Api.Extensions;

public static class EndpointExtensions
{
  public static WebApplication MapEndpoints(this WebApplication app)
  {
    app.MapGet("/version", () => TypedResults.Ok(new
    {
      Version = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "unknown"
    }));

    // Endpoint for load Balancers (Fast Liveness)
    app.MapHealthChecks("/health/live", new HealthCheckOptions
    {
      Predicate = _ => false
    });

    // Endpoint for full readiness (DB, Redis, and APIs are up)
    app.MapHealthChecks("/health/ready", new HealthCheckOptions
    {
      Predicate = (check) => check.Tags.Contains("ready"),
      ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });

    app.MapGroup("/api")
      .MapOrganizationEndpoints()
      .MapSiteEndpoints()
      .MapProfileEndpoints()
      .MapAuthEndpoints();

    return app;
  }
}