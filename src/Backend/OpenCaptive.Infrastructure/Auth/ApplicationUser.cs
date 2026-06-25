using Microsoft.AspNetCore.Identity;

namespace OpenCaptive.Infrastructure.Auth;

public sealed class ApplicationUser : IdentityUser<Guid>
{
  public bool IsActive { get; set; } = true;
  public DateTimeOffset? LastLoginAt { get; set; }
}