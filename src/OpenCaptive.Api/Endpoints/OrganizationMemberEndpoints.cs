using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OpenCaptive.Api.Authorization;
using OpenCaptive.Application.Organizations.Contracts;
using OpenCaptive.Application.Organizations.Models;
using OpenCaptive.Domain.Auth;

namespace OpenCaptive.Api.Endpoints;

public static class OrganizationMemberEndpoints
{
  public static IEndpointRouteBuilder MapOrganizationMemberEndpoints(this IEndpointRouteBuilder app)
  {
    var group = app.MapGroup("/organization/members").WithTags("Organizations");

    // No add-by-id — members only join via invitation (see InvitationEndpoints).
    group.MapGet(string.Empty, GetAllMembers).RequirePermission(Permissions.Members.Read);
    group.MapGet("/{memberId:guid}", GetOneMember).RequirePermission(Permissions.Members.Read);
    group.MapPatch("/{memberId:guid}", UpdateMember).RequirePermission(Permissions.Members.Read);
    group.MapDelete("/{memberId:guid}", RemoveMember).RequirePermission(Permissions.Members.Remove);

    return app;
  }

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
