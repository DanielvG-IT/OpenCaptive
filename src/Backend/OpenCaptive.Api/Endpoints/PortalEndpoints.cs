using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OpenCaptive.Api.Authorization;
using OpenCaptive.Api.Extensions;
using OpenCaptive.Application.Portals;
using OpenCaptive.Domain.Auth;

namespace OpenCaptive.Api.Endpoints;

public static class PortalEndpoints
{
  public static IEndpointRouteBuilder MapPortalEndpoints(this IEndpointRouteBuilder app)
  {
    var portalGroup = app.MapGroup("/networks/{networkId:guid}/portal")
        .RequireAuthorization()
        .WithTags("Portals");

    portalGroup.MapGet(string.Empty, GetPortal).RequirePermission(Permissions.Portals.Read);
    portalGroup.MapPost(string.Empty, CreatePortal).RequirePermission(Permissions.Portals.Create);
    portalGroup.MapPatch(string.Empty, UpdatePortal).RequirePermission(Permissions.Portals.Update);

    var versionGroup = app.MapGroup("/portals/{portalId:guid}/versions")
        .RequireAuthorization()
        .WithTags("Portals");

    versionGroup.MapGet(string.Empty, GetVersions).RequirePermission(Permissions.Portals.Read);
    versionGroup.MapPost(string.Empty, CreateVersion).RequirePermission(Permissions.Portals.Update);
    versionGroup.MapPost("/{versionId:guid}/publish", PublishVersion).RequirePermission(Permissions.Portals.Publish);

    return app;
  }

  private static async Task<Results<Ok<PortalDto>, ProblemHttpResult>> GetPortal(
    [FromRoute] Guid networkId,
    [FromServices] IPortalService service,
    CancellationToken cancellationToken)
  {
    var result = await service.GetAsync(networkId, cancellationToken);
    if (result.IsFailure)
    {
      return result.Error.ToProblem();
    }

    return TypedResults.Ok(result.Value);
  }

  private static async Task<Results<Ok<PortalDto>, ValidationProblem, ProblemHttpResult>> CreatePortal(
    [FromRoute] Guid networkId,
    [FromBody] CreatePortalInput input,
    [FromServices] IValidator<CreatePortalInput> validator,
    [FromServices] IPortalService service,
    CancellationToken cancellationToken)
  {
    var validation = await validator.ValidateAsync(input, cancellationToken);
    if (!validation.IsValid)
    {
      return TypedResults.ValidationProblem(validation.ToDictionary());
    }

    var result = await service.CreateAsync(networkId, input, cancellationToken);
    if (result.IsFailure)
    {
      return result.Error.ToProblem();
    }

    return TypedResults.Ok(result.Value);
  }

  private static async Task<Results<Ok<PortalDto>, ValidationProblem, ProblemHttpResult>> UpdatePortal(
    [FromRoute] Guid networkId,
    [FromBody] UpdatePortalInput input,
    [FromServices] IValidator<UpdatePortalInput> validator,
    [FromServices] IPortalService service,
    CancellationToken cancellationToken)
  {
    var validation = await validator.ValidateAsync(input, cancellationToken);
    if (!validation.IsValid)
    {
      return TypedResults.ValidationProblem(validation.ToDictionary());
    }

    var result = await service.UpdateAsync(networkId, input, cancellationToken);
    if (result.IsFailure)
    {
      return result.Error.ToProblem();
    }

    return TypedResults.Ok(result.Value);
  }

  private static async Task<Results<Ok<List<PortalVersionDto>>, ProblemHttpResult>> GetVersions(
    [FromRoute] Guid portalId,
    [FromServices] IPortalService service,
    CancellationToken cancellationToken)
  {
    var result = await service.GetVersionsAsync(portalId, cancellationToken);
    if (result.IsFailure)
    {
      return result.Error.ToProblem();
    }

    return TypedResults.Ok(result.Value);
  }

  private static async Task<Results<Ok<PortalVersionDto>, ValidationProblem, ProblemHttpResult>> CreateVersion(
    [FromRoute] Guid portalId,
    [FromBody] CreatePortalVersionInput input,
    [FromServices] IValidator<CreatePortalVersionInput> validator,
    [FromServices] IPortalService service,
    CancellationToken cancellationToken)
  {
    var validation = await validator.ValidateAsync(input, cancellationToken);
    if (!validation.IsValid)
    {
      return TypedResults.ValidationProblem(validation.ToDictionary());
    }

    var result = await service.CreateVersionAsync(portalId, input, cancellationToken);
    if (result.IsFailure)
    {
      return result.Error.ToProblem();
    }

    return TypedResults.Ok(result.Value);
  }

  private static async Task<Results<Ok<PortalDto>, ProblemHttpResult>> PublishVersion(
    [FromRoute] Guid portalId,
    [FromRoute] Guid versionId,
    [FromServices] IPortalService service,
    CancellationToken cancellationToken)
  {
    var result = await service.PublishVersionAsync(portalId, versionId, cancellationToken);
    if (result.IsFailure)
    {
      return result.Error.ToProblem();
    }

    return TypedResults.Ok(result.Value);
  }
}
