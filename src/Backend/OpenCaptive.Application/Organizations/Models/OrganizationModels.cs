namespace OpenCaptive.Application.Organizations.Models;

public sealed record CreateOrganizationInput(string Name, string Slug);

public sealed record OrganizationDto(Guid Id, string Name, string Slug);
