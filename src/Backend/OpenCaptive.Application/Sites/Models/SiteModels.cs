namespace OpenCaptive.Application.Sites.Models;

public sealed record CreateSiteInput(string Name, string Slug);
public sealed record UpdateSiteInput(string? Name, string? Slug);

public sealed record SiteDto(Guid Id, string Name, string Slug);
