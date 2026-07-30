using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OpenCaptive.Api.Authorization;
using OpenCaptive.Api.Extensions;
using OpenCaptive.Application.Networks;
using OpenCaptive.Domain.Auth;

namespace OpenCaptive.Api.Endpoints;

public static class NetworkEndpoints
{
  public static IEndpointRouteBuilder MapNetworkEndpoints(this IEndpointRouteBuilder app)
  {
    var group = app.MapGroup("/sites/{siteId:guid}/networks")
        .RequireAuthorization()
        .WithTags("Networks");

    group.MapPost(string.Empty, CreateNetwork).RequirePermission(Permissions.Networks.Create);
    group.MapGet(string.Empty, GetAllNetworks).RequirePermission(Permissions.Networks.ReadAll);
    group.MapGet("/{id:guid}", GetOneNetwork).RequirePermission(Permissions.Networks.ReadOne).WithName("GetNetworkById");
    group.MapPatch("/{id:guid}", UpdateNetwork).RequirePermission(Permissions.Networks.Update);
    group.MapDelete("/{id:guid}", DeleteNetwork).RequirePermission(Permissions.Networks.Delete);

    return app;
  }

  private static async Task<Results<CreatedAtRoute<NetworkDto>, ValidationProblem, ProblemHttpResult>> CreateNetwork(
    [FromRoute] Guid siteId,
    [FromBody] CreateNetworkInput input,
    [FromServices] IValidator<CreateNetworkInput> validator,
    [FromServices] INetworkService service,
    CancellationToken cancellationToken)
  {
    var validation = await validator.ValidateAsync(input, cancellationToken);
    if (!validation.IsValid)
    {
      return TypedResults.ValidationProblem(validation.ToDictionary());
    }

    var result = await service.CreateAsync(siteId, input, cancellationToken);
    if (result.IsFailure)
    {
      return result.Error.ToProblem();
    }

    return TypedResults.CreatedAtRoute(result.Value, "GetNetworkById", new { siteId, id = result.Value.Id });
  }

  private static async Task<Results<Ok<List<NetworkDto>>, ProblemHttpResult>> GetAllNetworks(
    [FromRoute] Guid siteId,
    [FromServices] INetworkService service,
    CancellationToken cancellationToken)
  {
    var result = await service.GetAllAsync(siteId, cancellationToken);
    if (result.IsFailure)
    {
      return result.Error.ToProblem();
    }

    return TypedResults.Ok(result.Value);
  }

  private static async Task<Results<Ok<NetworkDto>, ProblemHttpResult>> GetOneNetwork(
    [FromRoute] Guid siteId,
    [FromRoute] Guid id,
    [FromServices] INetworkService service,
    CancellationToken cancellationToken)
  {
    var result = await service.GetOneByIdAsync(siteId, id, cancellationToken);
    if (result.IsFailure)
    {
      return result.Error.ToProblem();
    }

    return TypedResults.Ok(result.Value);
  }

  private static async Task<Results<Ok<NetworkDto>, ValidationProblem, ProblemHttpResult>> UpdateNetwork(
    [FromRoute] Guid siteId,
    [FromRoute] Guid id,
    [FromBody] UpdateNetworkInput input,
    [FromServices] IValidator<UpdateNetworkInput> validator,
    [FromServices] INetworkService service,
    CancellationToken cancellationToken)
  {
    var validation = await validator.ValidateAsync(input, cancellationToken);
    if (!validation.IsValid)
    {
      return TypedResults.ValidationProblem(validation.ToDictionary());
    }

    var result = await service.UpdateAsync(siteId, id, input, cancellationToken);
    if (result.IsFailure)
    {
      return result.Error.ToProblem();
    }

    return TypedResults.Ok(result.Value);
  }

  private static async Task<Results<NoContent, ProblemHttpResult>> DeleteNetwork(
    [FromRoute] Guid siteId,
    [FromRoute] Guid id,
    [FromServices] INetworkService service,
    CancellationToken cancellationToken)
  {
    var result = await service.DeleteAsync(siteId, id, cancellationToken);
    if (result.IsFailure)
    {
      return result.Error.ToProblem();
    }

    return TypedResults.NoContent();
  }
}
