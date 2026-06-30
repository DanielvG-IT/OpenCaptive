using OpenCaptive.Api.Extensions;
using OpenCaptive.Application.Common.Contracts;

namespace OpenCaptive.Api.Authentication;

public sealed class ClaimsCurrentUser : ICurrentUser
{
  public ClaimsCurrentUser(IHttpContextAccessor httpContextAccessor)
  {
    var claims = httpContextAccessor.HttpContext?.User ?? throw new InvalidOperationException("No current HttpContext.");

    UserId = claims.GetUserId();
    OrganizationId = claims.GetOrganizationId();
    Email = claims.GetEmail();
  }

  public Guid UserId { get; }
  public Guid OrganizationId { get; }
  public string Email { get; }
}