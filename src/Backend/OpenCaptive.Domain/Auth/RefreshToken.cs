using OpenCaptive.Domain.Common;

namespace OpenCaptive.Domain.Auth;

public sealed class RefreshToken : AuditableEntity
{
  public Guid UserId { get; private set; }
  public string TokenHash { get; private set; } = default!;
  public string SecurityStamp { get; private set; } = default!;
  public DateTimeOffset ExpiresAt { get; private set; }
  public DateTimeOffset? RevokedAt { get; private set; }
  public Guid FamilyId { get; private set; }

  private RefreshToken() { } // Required by EF Core for materialization.

  public static RefreshToken Create(Guid userId, string tokenHash, string securityStamp, DateTimeOffset expiresAt, Guid familyId)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(tokenHash);
    ArgumentException.ThrowIfNullOrWhiteSpace(securityStamp);
    ArgumentOutOfRangeException.ThrowIfEqual(userId, Guid.Empty);
    ArgumentOutOfRangeException.ThrowIfEqual(familyId, Guid.Empty);
    ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(expiresAt, DateTimeOffset.UtcNow);

    return new RefreshToken
    {
      Id = Guid.CreateVersion7(),
      TokenHash = tokenHash,
      SecurityStamp = securityStamp,
      UserId = userId,
      FamilyId = familyId,
      ExpiresAt = expiresAt,
      RevokedAt = null
    };
  }

  public void Revoke()
  {
    RevokedAt = DateTimeOffset.UtcNow;
  }

  public bool IsActive => !IsExpired && !IsRevoked;
  public bool IsExpired => ExpiresAt <= DateTimeOffset.UtcNow;
  public bool IsRevoked => RevokedAt is not null;
}
