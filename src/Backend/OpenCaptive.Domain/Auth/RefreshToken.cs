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

  // Required by EF Core for materialization.
  private RefreshToken()
  {
  }

  public static RefreshToken Create(Guid userId, string tokenHash, string securityStamp, DateTimeOffset expiresAt, Guid familyId)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(tokenHash);
    ArgumentException.ThrowIfNullOrWhiteSpace(securityStamp);

    if (userId == Guid.Empty)
      throw new ArgumentException("User ID must not be empty.", nameof(userId));

    if (familyId == Guid.Empty)
      throw new ArgumentException("Family ID must not be empty.", nameof(familyId));

    if (expiresAt <= DateTimeOffset.UtcNow)
      throw new ArgumentException("Expiry time must be in the future.", nameof(expiresAt));

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
