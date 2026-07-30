using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OpenCaptive.Api.Authorization;
using OpenCaptive.Api.Extensions;
using OpenCaptive.Application.Organizations.Contracts;
using OpenCaptive.Application.Organizations.Models;
using OpenCaptive.Domain.Auth;

namespace OpenCaptive.Api.Endpoints;

public static class OrganizationEndpoints
{
  public static IEndpointRouteBuilder MapOrganizationEndpoints(this IEndpointRouteBuilder app)
  {
    // Singular: there is only one organization visible to the current user, derived from
    // ICurrentUser.OrganizationId — same reasoning as /profile, not /profiles.
    var group = app.MapGroup("/organization").WithTags("Organizations");

    group.MapGet(string.Empty, GetOrganization).RequirePermission(Permissions.Organizations.Read);
    group.MapPatch(string.Empty, UpdateOrganization).RequirePermission(Permissions.Organizations.Update);
    group.MapDelete(string.Empty, DeleteOrganization).RequirePermission(Permissions.Organizations.Delete);

    return app;
  }

  private static async Task<Results<Ok<OrganizationDto>, ProblemHttpResult>> GetOrganization(
    [FromServices] IOrganizationService service,
    CancellationToken cancellationToken)
  {
    var result = await service.GetAsync(cancellationToken);
    if (result.IsFailure)
    {
      return result.Error.ToProblem();
    }

    return TypedResults.Ok(result.Value);
  }

  private static async Task<Results<Ok<OrganizationDto>, ProblemHttpResult>> UpdateOrganization(
    [FromBody] UpdateOrganizationInput input,
    [FromServices] IOrganizationService service,
    CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }

  private static async Task<Results<NoContent, ProblemHttpResult>> DeleteOrganization(
    [FromServices] IOrganizationService service,
    CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }
}
