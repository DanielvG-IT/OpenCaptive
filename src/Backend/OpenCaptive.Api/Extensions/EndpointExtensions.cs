using System.Reflection;
using OpenCaptive.Api.Endpoints;

namespace OpenCaptive.Api.Extensions;

public static class EndpointExtensions
{
  public static WebApplication MapEndpoints(this WebApplication app, string prefix)
  {
    app.MapGet("/version", () => TypedResults.Ok(new
    {
      Version = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "unknown"
    }));

    app.MapGroup(prefix)
      .MapOrganizationEndpoints()
      .MapAuthEndpoints();

    return app;
  }
}