namespace OpenCaptive.Application.Sites;

public sealed record CreateSiteInput(string Name, string Slug, string TimeZone, string? Description);
public sealed record UpdateSiteInput(string? Name, string? Slug, string? TimeZone, string? Description);

public sealed record SiteDto(Guid Id, string Name, string Slug, string? Description, string TimeZone, bool IsEnabled);
public sealed record SiteSummaryDto(Guid Id, string Name, string Slug, bool IsEnabled);
