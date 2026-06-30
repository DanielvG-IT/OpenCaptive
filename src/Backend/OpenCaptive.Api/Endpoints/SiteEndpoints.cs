using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OpenCaptive.Api.Authorization;
using OpenCaptive.Application.Sites.Contracts;
using OpenCaptive.Application.Sites.Models;
using OpenCaptive.Domain.Auth;

namespace OpenCaptive.Api.Endpoints;

public static class SiteEndpoints
{
  public static IEndpointRouteBuilder MapSiteEndpoints(this IEndpointRouteBuilder app)
  {
    var group = app.MapGroup("/sites")
        .WithTags("Sites")
        .RequireAuthorization();

    // Site Management 
    group.MapPost("/", CreateSite).RequirePermission(Permissions.Sites.Read);
    group.MapGet("/{id:guid}", GetSite);
    group.MapPatch("/{id:guid}", UpdateSite);
    group.MapDelete("/{id:guid}", DeleteSite);

    return app;
  }

  private static async Task<Results<Ok<SiteDto>, ProblemHttpResult>> GetSite(
    [FromRoute] Guid id,
    [FromServices] ISiteService service,
    CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }

  private static async Task<Results<CreatedAtRoute<SiteDto>, ProblemHttpResult>> CreateSite(
    [FromRoute] Guid id,
    [FromServices] ISiteService service,
    CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }

  private static async Task<Results<Ok<SiteDto>, ProblemHttpResult>> UpdateSite(
    [FromRoute] Guid id,
    [FromServices] ISiteService service,
    CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }

  private static async Task<Results<NoContent, ProblemHttpResult>> DeleteSite(
    [FromRoute] Guid id,
    [FromServices] ISiteService service,
    CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }
}
