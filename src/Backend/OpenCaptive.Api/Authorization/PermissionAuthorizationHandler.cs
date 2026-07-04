using Microsoft.AspNetCore.Authorization;
using OpenCaptive.Domain.Auth;
using OpenCaptive.Domain.Organizations;
using OpenCaptive.Infrastructure.Auth;

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

    context.Succeed(requirement);
    return Task.CompletedTask;
  }
}
