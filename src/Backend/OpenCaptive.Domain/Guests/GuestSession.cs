using OpenCaptive.Domain.Common;

namespace OpenCaptive.Domain.Guests;

public sealed class GuestSession : AuditableEntity
{
  public Guid SiteId { get; private set; }
  public Guid NetworkId { get; private set; }
  public string ClientMacAddress { get; private set; } = null!;
  public string? ClientIpAddress { get; private set; }
  public DateTimeOffset StartedAt { get; private set; }
  public DateTimeOffset? AuthenticatedAt { get; private set; }
  public DateTimeOffset? EndedAt { get; private set; }
  public bool IsAuthenticated { get; private set; }

  private GuestSession() { } // Required by EF Core for materialization.

  public static GuestSession Create(Guid siteId, Guid networkId, string clientMacAddress, string? clientIpAddress)
  {
    ArgumentOutOfRangeException.ThrowIfEqual(siteId, Guid.Empty);
    ArgumentOutOfRangeException.ThrowIfEqual(networkId, Guid.Empty);
    ArgumentException.ThrowIfNullOrWhiteSpace(clientMacAddress);

    return new GuestSession
    {
      Id = NewId(),
      SiteId = siteId,
      NetworkId = networkId,
      ClientMacAddress = clientMacAddress,
      ClientIpAddress = clientIpAddress,
      StartedAt = DateTimeOffset.UtcNow,
    };
  }

  public void Authenticate()
  {
    IsAuthenticated = true;
    AuthenticatedAt = DateTimeOffset.UtcNow;
  }

  public void EndSession()
  {
    EndedAt = DateTimeOffset.UtcNow;
  }
}