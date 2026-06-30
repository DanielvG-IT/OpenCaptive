using OpenCaptive.Domain.Organizations;

namespace OpenCaptive.Application.Organizations.Models;

public sealed record UpdateOrganizationInput(string? Name, string? Slug);
public sealed record AddMemberInput(string Email);

public sealed record OrganizationDto(Guid Id, string Name, string Slug);
public sealed record MemberDto(string FirstName, string LastName, OrganizationRole Role);