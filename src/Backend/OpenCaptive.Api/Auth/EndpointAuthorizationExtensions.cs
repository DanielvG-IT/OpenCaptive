using Microsoft.AspNetCore.Authorization;

namespace OpenCaptive.Api.Auth;

public static class EndpointAuthorizationExtensions
{
  public static TBuilder RequirePermission<TBuilder>(this TBuilder builder, string permission) where TBuilder : IEndpointConventionBuilder
  {
    return builder.RequireAuthorization(new AuthorizationPolicyBuilder()
      .RequireAuthenticatedUser()
      .AddRequirements(new PermissionRequirement(permission))
      .Build());
  }
}
