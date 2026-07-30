using OpenCaptive.Domain.Organizations;

namespace OpenCaptive.Application.Auth.Contracts;

public interface IAccessTokenGenerator
{
  AccessTokenResponse Generate(AccessTokenRequest request);
}

public sealed record AccessTokenRequest(Guid UserId, string Email, Guid? OrganizationId, OrganizationRole? OrganizationRole);
public sealed record AccessTokenResponse(string Token, DateTimeOffset ExpiresAt);