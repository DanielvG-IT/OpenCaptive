namespace OpenCaptive.Application.Integrations;

public sealed record CreateSiteIntegrationInput(string Provider, string DisplayName);
public sealed record UpdateSiteIntegrationInput(string? DisplayName, bool? IsEnabled);

public sealed record SiteIntegrationDto(
  Guid Id,
  Guid SiteId,
  string Provider,
  string DisplayName,
  bool IsEnabled,
  DateTimeOffset? LastSuccessfulConnectionAt,
  DateTimeOffset? LastCapabilityRefreshAt,
  string? LastError);
