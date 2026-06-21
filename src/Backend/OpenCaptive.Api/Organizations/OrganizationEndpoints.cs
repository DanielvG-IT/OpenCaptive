using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OpenCaptive.Api.Authorization;
using OpenCaptive.Api.Extensions;
using OpenCaptive.Application.Organizations.Contracts;
using OpenCaptive.Application.Organizations.Models;
using OpenCaptive.Domain.Auth;

namespace OpenCaptive.Api.Organizations;

public static class OrganizationEndpoints
{
  public static IEndpointRouteBuilder MapOrganizationEndpoints(this IEndpointRouteBuilder app)
  {
    var group = app.MapGroup("/organizations").WithTags("Organizations");

    group.MapPost("/", CreateOrganization).WithName("CreateOrganization");
    group.MapGet("/{id:guid}", GetOrganization).WithName("GetOrganization").RequirePermission(Permissions.Organizations.Read);
    // group.MapPatch("/{id:guid}", UpdateOrganization);
    // group.MapDelete("/{id:guid}", DeleteOrganization);

    return app;
  }

  private static async Task<Results<Created<OrganizationDto>, ValidationProblem, ProblemHttpResult>> CreateOrganization(
    [FromBody] CreateOrganizationInput input,
    [FromServices] IValidator<CreateOrganizationInput> validator,
    [FromServices] IOrganizationService service,
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

    return TypedResults.Created($"/organizations/{result.Value.Id}", result.Value);
  }

  private static async Task<Results<Ok<OrganizationDto>, ProblemHttpResult>> GetOrganization(
    [FromRoute] Guid id,
    [FromServices] IOrganizationService service,
    CancellationToken cancellationToken)
  {
    var result = await service.GetAsync(id, cancellationToken);
    if (result.IsFailure)
    {
      return result.Error.ToProblem();
    }

    return TypedResults.Ok(result.Value);
  }
}
