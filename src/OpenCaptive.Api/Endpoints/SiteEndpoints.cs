using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OpenCaptive.Api.Authorization;
using OpenCaptive.Api.Extensions;
using OpenCaptive.Application.Sites;
using OpenCaptive.Domain.Auth;

namespace OpenCaptive.Api.Endpoints;

public static class SiteEndpoints
{
  public static IEndpointRouteBuilder MapSiteEndpoints(this IEndpointRouteBuilder app)
  {
    var group = app.MapGroup("/sites")
        .RequireAuthorization()
        .WithTags("Sites");

    // Site Management 
    group.MapPost(string.Empty, CreateSite).RequirePermission(Permissions.Sites.Create);
    group.MapGet(string.Empty, GetAllSites).RequirePermission(Permissions.Sites.ReadAll);
    group.MapGet("/{id:guid}", GetOneSite).RequirePermission(Permissions.Sites.ReadOne).WithName("GetSiteById");
    group.MapPatch("/{id:guid}", UpdateSite).RequirePermission(Permissions.Sites.Update);
    group.MapDelete("/{id:guid}", DeleteSite).RequirePermission(Permissions.Sites.Delete);

    return app;
  }

  private static async Task<Results<CreatedAtRoute<SiteDto>, ValidationProblem, ProblemHttpResult>> CreateSite(
    [FromBody] CreateSiteInput input,
    [FromServices] IValidator<CreateSiteInput> validator,
    [FromServices] ISiteService service,
    CancellationToken cancellationToken)
  {
    var validation = await validator.ValidateAsync(input, cancellationToken);
    if (!validation.IsValid)
    {
      return TypedResults.ValidationProblem(validation.ToDictionary());
    }

    var result = await service.CreateAsync(input, cancellationToken);
    if (result.IsFailure)
    {
      return result.Error.ToProblem();
    }

    return TypedResults.CreatedAtRoute(result.Value, "GetSiteById", new { id = result.Value.Id });
  }

  private static async Task<Results<Ok<List<SiteSummaryDto>>, ProblemHttpResult>> GetAllSites(
    [FromServices] ISiteService service,
    CancellationToken cancellationToken)
  {
    var result = await service.GetAllAsync(cancellationToken);
    if (result.IsFailure)
    {
      return result.Error.ToProblem();
    }

    return TypedResults.Ok(result.Value);
  }

  private static async Task<Results<Ok<SiteDto>, ProblemHttpResult>> GetOneSite(
    [FromRoute] Guid id,
    [FromServices] ISiteService service,
    CancellationToken cancellationToken)
  {
    var result = await service.GetOneByIdAsync(id, cancellationToken);
    if (result.IsFailure)
    {
      return result.Error.ToProblem();
    }

    return TypedResults.Ok(result.Value);
  }

  private static async Task<Results<Ok<SiteDto>, ValidationProblem, ProblemHttpResult>> UpdateSite(
    [FromRoute] Guid id,
    [FromBody] UpdateSiteInput input,
    [FromServices] IValidator<UpdateSiteInput> validator,
    [FromServices] ISiteService service,
    CancellationToken cancellationToken)
  {
    var validation = await validator.ValidateAsync(input, cancellationToken);
    if (!validation.IsValid)
    {
      return TypedResults.ValidationProblem(validation.ToDictionary());
    }

    var result = await service.UpdateAsync(id, input, cancellationToken);
    if (result.IsFailure)
    {
      return result.Error.ToProblem();
    }

    return TypedResults.Ok(result.Value);
  }

  private static async Task<Results<NoContent, ProblemHttpResult>> DeleteSite(
    [FromRoute] Guid id,
    [FromServices] ISiteService service,
    CancellationToken cancellationToken)
  {
    var result = await service.DeleteAsync(id, cancellationToken);
    if (result.IsFailure)
    {
      return result.Error.ToProblem();
    }

    return TypedResults.NoContent();
  }
}
