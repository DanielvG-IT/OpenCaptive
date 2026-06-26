using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace OpenCaptive.Infrastructure.Auth;

public sealed class ApplicationUser : IdentityUser<Guid>
{
  [NotMapped]
  public string FullName => $"{FirstName} {LastName}";

  public required string FirstName { get; set; }
  public required string LastName { get; set; }

  public bool IsActive { get; set; } = true;
  public DateTimeOffset? LastLoginAt { get; set; }
}