using OpenCaptive.Api.Endpoints;

namespace OpenCaptive.Api.Extensions;

public static class EndpointExtensions
{
  public static WebApplication MapEndpoints(this WebApplication app)
  {
    app.MapOrganizationEndpoints();

    return app;
  }
}
