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
    var group = app.MapGroup("/organizations").WithTags("Organizations");

    // Organization Management
    group.MapGet("/{id:guid}", GetOrganization).RequirePermission(Permissions.Organizations.Read);
    group.MapPatch("/{id:guid}", UpdateOrganization).RequirePermission(Permissions.Organizations.Update);
    group.MapDelete("/{id:guid}", DeleteOrganization).RequirePermission(Permissions.Organizations.Delete);

    // Member Management
    group.MapGet("/{id:guid}/members", GetMembers).RequirePermission(Permissions.Members.Read);
    group.MapPost("/{id:guid}/members", AddMember).RequirePermission(Permissions.Members.Add);
    group.MapDelete("/{id:guid}/members/{memberId:guid}", RemoveMember).RequirePermission(Permissions.Members.Remove);

    return app;
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

  private static async Task<Results<Ok<OrganizationDto>, ProblemHttpResult>> UpdateOrganization(
    [FromRoute] Guid id,
    [FromBody] UpdateOrganizationInput input,
    [FromServices] IOrganizationService service,
    CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }

  private static async Task<Results<NoContent, ProblemHttpResult>> DeleteOrganization(
    [FromRoute] Guid id,
    [FromServices] IOrganizationService service,
    CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }

  private static async Task<Results<Ok<List<MemberDto>>, ProblemHttpResult>> GetMembers(
    [FromRoute] Guid id,
    [FromServices] IOrganizationService service,
    CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }

  private static async Task<Results<NoContent, ProblemHttpResult>> AddMember(
    [FromRoute] Guid id,
    [FromBody] AddMemberInput input,
    [FromServices] IOrganizationService service,
    CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }

  private static async Task<Results<NoContent, ProblemHttpResult>> RemoveMember(
    [FromRoute] Guid id,
    [FromRoute] Guid memberId,
    [FromServices] IOrganizationService service,
    CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }
}
