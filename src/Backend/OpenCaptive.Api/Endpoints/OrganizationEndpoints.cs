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
    group.MapGet(string.Empty, GetOrganization).RequirePermission(Permissions.Organizations.Read);
    group.MapPatch(string.Empty, UpdateOrganization).RequirePermission(Permissions.Organizations.Update);
    group.MapDelete(string.Empty, DeleteOrganization).RequirePermission(Permissions.Organizations.Delete);

    // Member Management
    group.MapGet("/members", GetAllMembers).RequirePermission(Permissions.Members.Read);
    group.MapGet("/members/{memberId:guid}", GetOneMember).RequirePermission(Permissions.Members.Read);
    group.MapPatch("/members/{memberId:guid}", UpdateMember).RequirePermission(Permissions.Members.Read);
    group.MapDelete("/members/{memberId:guid}", RemoveMember).RequirePermission(Permissions.Members.Remove);

    // Invitation Management moved to InvitationEndpoints.MapInvitationEndpoints()
    // (/organizations/invitations) — Invitation has its own lifecycle/token/expiry
    // independent of Organization, so it gets its own service.

    return app;
  }

  //* ===============
  //*  Organizations
  //* ===============
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


  //* ===============
  //*    Members
  //* ===============
  private static async Task<Results<Ok<List<MemberDto>>, ProblemHttpResult>> GetAllMembers(
    [FromServices] IOrganizationService service,
    CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }

  private static async Task<Results<Ok<MemberDto>, ProblemHttpResult>> GetOneMember(
    [FromRoute] Guid memberId,
    [FromServices] IOrganizationService service,
    CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }

  private static async Task<Results<NoContent, ProblemHttpResult>> UpdateMember(
    [FromRoute] Guid memberId,
    [FromBody] UpdateMemberInput input,
    [FromServices] IOrganizationService service,
    CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }

  private static async Task<Results<NoContent, ProblemHttpResult>> RemoveMember(
    [FromRoute] Guid memberId,
    [FromServices] IOrganizationService service,
    CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }
}
