using OpenCaptive.Domain.Common;

namespace OpenCaptive.Domain.Integrations;

public sealed class SiteIntegration : AuditableEntity
{
  public Guid SiteId { get; private set; }
  public string Provider { get; private set; } = null!; // "unifi", "omada", "mikrotik"
  public string DisplayName { get; private set; } = null!;
  public bool IsEnabled { get; private set; } = true;
  public DateTimeOffset? LastSuccessfulConnectionAt { get; private set; }
  public DateTimeOffset? LastCapabilityRefreshAt { get; private set; }
  public string? LastError { get; private set; }

  private SiteIntegration() { } // Required by EF Core for materialization.

  public static SiteIntegration Create(Guid siteId, string provider, string displayName)
  {
    ArgumentOutOfRangeException.ThrowIfEqual(siteId, Guid.Empty);
    ArgumentException.ThrowIfNullOrWhiteSpace(provider);
    ArgumentException.ThrowIfNullOrWhiteSpace(displayName);

    return new SiteIntegration
    {
      Id = NewId(),
      SiteId = siteId,
      Provider = provider,
      DisplayName = displayName,
      IsEnabled = true,
    };
  }

  public void MarkConnected()
  {
    LastSuccessfulConnectionAt = DateTimeOffset.UtcNow;
  }

  public void MarkConnectionFailed(string error)
  {
    LastError = error;
  }

  public void MarkCapabilitiesRefreshed()
  {
    LastCapabilityRefreshAt = DateTimeOffset.UtcNow;
  }

  public void Enable()
  {
    IsEnabled = true;
  }

  public void Disable()
  {
    IsEnabled = false;
  }
}