using System.Reflection;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using OpenCaptive.Api.Endpoints;

namespace OpenCaptive.Api.Extensions;

public static class EndpointExtensions
{
  public static WebApplication MapEndpoints(this WebApplication app)
  {
    // Build info: reports the deployed assembly's informational version. Public and
    // unauthenticated so a deploy can be verified from outside; exposes only the version
    // string, nothing sensitive.
    app.MapGet("/version", () => TypedResults.Ok(new
    {
      Version = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "unknown"
    }));

    // Liveness: is the process up and answering? Runs no checks. Public and cheap —
    // the platform load balancer probes this, so it must stay anonymous.
    app.MapHealthChecks("/health/live", new HealthCheckOptions
    {
      Predicate = _ => false
    });

    // Readiness: are the dependencies tagged "ready" healthy enough to serve traffic?
    // Public for orchestrator probes, but STATUS-ONLY — no per-check detail is exposed,
    // so an anonymous caller learns nothing about component topology or failures.
    app.MapHealthChecks("/health/ready", new HealthCheckOptions
    {
      Predicate = (check) => check.Tags.Contains("ready")
    });

    // A detailed diagnostics endpoint (full per-check JSON) is intentionally omitted: it
    // leaks component names and exception text, and RequireAuthorization alone is too broad
    // for ops-only data. Add it later behind a dedicated ops permission, once one exists.
    // app.MapHealthChecks("/health/detail", new HealthCheckOptions
    // {
    //   ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    // })

    // Business API surface: all authenticated resource endpoints live under /api, each
    // module mapping its own routes and applying its own authorization (see RequirePermission).
    app.MapGroup("/api")
      .MapOrganizationEndpoints()
      .MapInvitationEndpoints()
      .MapSiteEndpoints()
      .MapNetworkEndpoints()
      .MapIntegrationEndpoints()
      .MapPortalEndpoints()
      .MapProfileEndpoints()
      .MapAuthEndpoints();

    return app;
  }
}