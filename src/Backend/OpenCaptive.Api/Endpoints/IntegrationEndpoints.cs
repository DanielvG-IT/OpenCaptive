using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OpenCaptive.Api.Authorization;
using OpenCaptive.Api.Extensions;
using OpenCaptive.Application.Integrations;
using OpenCaptive.Domain.Auth;

namespace OpenCaptive.Api.Endpoints;

public static class IntegrationEndpoints
{
  public static IEndpointRouteBuilder MapIntegrationEndpoints(this IEndpointRouteBuilder app)
  {
    var group = app.MapGroup("/sites/{siteId:guid}/integrations")
        .RequireAuthorization()
        .WithTags("Integrations");

    group.MapPost(string.Empty, CreateIntegration).RequirePermission(Permissions.Integrations.Create);
    group.MapGet(string.Empty, GetAllIntegrations).RequirePermission(Permissions.Integrations.ReadAll);
    group.MapGet("/{id:guid}", GetOneIntegration).RequirePermission(Permissions.Integrations.ReadOne).WithName("GetIntegrationById");
    group.MapPatch("/{id:guid}", UpdateIntegration).RequirePermission(Permissions.Integrations.Update);
    group.MapDelete("/{id:guid}", DeleteIntegration).RequirePermission(Permissions.Integrations.Delete);

    group.MapPost("/{id:guid}/connect", ConnectIntegration).RequirePermission(Permissions.Integrations.Connect);
    group.MapPost("/{id:guid}/disconnect", DisconnectIntegration).RequirePermission(Permissions.Integrations.Disconnect);
    group.MapPost("/{id:guid}/sync-networks", SyncNetworks).RequirePermission(Permissions.Integrations.SyncNetworks);
    group.MapPost("/{id:guid}/refresh-capabilities", RefreshCapabilities).RequirePermission(Permissions.Integrations.RefreshCapabilities);

    return app;
  }

  private static async Task<Results<CreatedAtRoute<SiteIntegrationDto>, ValidationProblem, ProblemHttpResult>> CreateIntegration(
    [FromRoute] Guid siteId,
    [FromBody] CreateSiteIntegrationInput input,
    [FromServices] IValidator<CreateSiteIntegrationInput> validator,
    [FromServices] ISiteIntegrationService service,
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

    return TypedResults.CreatedAtRoute(result.Value, "GetIntegrationById", new { siteId, id = result.Value.Id });
  }

  private static async Task<Results<Ok<List<SiteIntegrationDto>>, ProblemHttpResult>> GetAllIntegrations(
    [FromRoute] Guid siteId,
    [FromServices] ISiteIntegrationService service,
    CancellationToken cancellationToken)
  {
    var result = await service.GetAllAsync(siteId, cancellationToken);
    if (result.IsFailure)
    {
      return result.Error.ToProblem();
    }

    return TypedResults.Ok(result.Value);
  }

  private static async Task<Results<Ok<SiteIntegrationDto>, ProblemHttpResult>> GetOneIntegration(
    [FromRoute] Guid siteId,
    [FromRoute] Guid id,
    [FromServices] ISiteIntegrationService service,
    CancellationToken cancellationToken)
  {
    var result = await service.GetOneByIdAsync(siteId, id, cancellationToken);
    if (result.IsFailure)
    {
      return result.Error.ToProblem();
    }

    return TypedResults.Ok(result.Value);
  }

  private static async Task<Results<Ok<SiteIntegrationDto>, ValidationProblem, ProblemHttpResult>> UpdateIntegration(
    [FromRoute] Guid siteId,
    [FromRoute] Guid id,
    [FromBody] UpdateSiteIntegrationInput input,
    [FromServices] IValidator<UpdateSiteIntegrationInput> validator,
    [FromServices] ISiteIntegrationService service,
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

  private static async Task<Results<NoContent, ProblemHttpResult>> DeleteIntegration(
    [FromRoute] Guid siteId,
    [FromRoute] Guid id,
    [FromServices] ISiteIntegrationService service,
    CancellationToken cancellationToken)
  {
    var result = await service.DeleteAsync(siteId, id, cancellationToken);
    if (result.IsFailure)
    {
      return result.Error.ToProblem();
    }

    return TypedResults.NoContent();
  }

  private static async Task<Results<Ok<SiteIntegrationDto>, ProblemHttpResult>> ConnectIntegration(
    [FromRoute] Guid siteId,
    [FromRoute] Guid id,
    [FromServices] ISiteIntegrationService service,
    CancellationToken cancellationToken)
  {
    var result = await service.ConnectAsync(siteId, id, cancellationToken);
    if (result.IsFailure)
    {
      return result.Error.ToProblem();
    }

    return TypedResults.Ok(result.Value);
  }

  private static async Task<Results<Ok<SiteIntegrationDto>, ProblemHttpResult>> DisconnectIntegration(
    [FromRoute] Guid siteId,
    [FromRoute] Guid id,
    [FromServices] ISiteIntegrationService service,
    CancellationToken cancellationToken)
  {
    var result = await service.DisconnectAsync(siteId, id, cancellationToken);
    if (result.IsFailure)
    {
      return result.Error.ToProblem();
    }

    return TypedResults.Ok(result.Value);
  }

  private static async Task<Results<Ok<SiteIntegrationDto>, ProblemHttpResult>> SyncNetworks(
    [FromRoute] Guid siteId,
    [FromRoute] Guid id,
    [FromServices] ISiteIntegrationService service,
    CancellationToken cancellationToken)
  {
    var result = await service.SyncNetworksAsync(siteId, id, cancellationToken);
    if (result.IsFailure)
    {
      return result.Error.ToProblem();
    }

    return TypedResults.Ok(result.Value);
  }

  private static async Task<Results<Ok<SiteIntegrationDto>, ProblemHttpResult>> RefreshCapabilities(
    [FromRoute] Guid siteId,
    [FromRoute] Guid id,
    [FromServices] ISiteIntegrationService service,
    CancellationToken cancellationToken)
  {
    var result = await service.RefreshCapabilitiesAsync(siteId, id, cancellationToken);
    if (result.IsFailure)
    {
      return result.Error.ToProblem();
    }

    return TypedResults.Ok(result.Value);
  }
}
