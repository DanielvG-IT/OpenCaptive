using Microsoft.AspNetCore.Authorization;

namespace OpenCaptive.Api.Auth;

public sealed class PermissionRequirement(string permission) : IAuthorizationRequirement
{
  public string Permission { get; } = permission;
}
