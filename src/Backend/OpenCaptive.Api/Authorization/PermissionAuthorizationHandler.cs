using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using OpenCaptive.Domain.Auth;
using OpenCaptive.Domain.Organizations;

namespace OpenCaptive.Api.Authorization;

public sealed class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
  protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
  {
    var roleClaim = context.User.FindFirst(OrganizationClaimTypes.OrganizationRole)?.Value;
    var orgIdClaim = context.User.FindFirst(OrganizationClaimTypes.OrganizationId)?.Value;

    if (roleClaim is null || orgIdClaim is null || !Enum.TryParse<OrganizationRole>(roleClaim, out var role) || !RolePermissions.RoleHasPermission(role, requirement.Permission))
    {
      return Task.CompletedTask;
    }

    // Single-org for now: if the route targets a specific organization, it must be the caller's own.
    if (context.Resource is HttpContext httpContext && httpContext.Request.RouteValues.TryGetValue("id", out var routeId) && routeId?.ToString() != orgIdClaim)
    {
      return Task.CompletedTask;
    }

    context.Succeed(requirement);
    return Task.CompletedTask;
  }
}
