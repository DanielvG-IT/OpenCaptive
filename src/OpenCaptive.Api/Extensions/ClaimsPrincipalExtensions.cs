using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using OpenCaptive.Domain.Organizations;
using OpenCaptive.Infrastructure.Auth;

namespace OpenCaptive.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        return Guid.Parse(user.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? throw new UnauthorizedAccessException());
    }

    public static string GetEmail(this ClaimsPrincipal user)
    {
        return user.FindFirstValue(JwtRegisteredClaimNames.Email)
            ?? throw new UnauthorizedAccessException();
    }

    public static Guid GetOrganizationId(this ClaimsPrincipal user)
    {
        return Guid.Parse(user.FindFirstValue(OrganizationClaimTypes.OrganizationId)
            ?? throw new UnauthorizedAccessException());
    }

    public static OrganizationRole GetOrganizationRole(this ClaimsPrincipal user)
    {
        var value = user.FindFirstValue(OrganizationClaimTypes.OrganizationRole)
            ?? throw new UnauthorizedAccessException();

        return Enum.Parse<OrganizationRole>(value);
    }
}