using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OpenCaptive.Domain.Auth;
using OpenCaptive.Domain.Common;
using OpenCaptive.Domain.Organizations;
using OpenCaptive.Infrastructure.Auth;

namespace OpenCaptive.Infrastructure.Persistence;

public sealed class OpenCaptiveDbContext(DbContextOptions<OpenCaptiveDbContext> options) : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>(options)
{
  public DbSet<Organization> Organizations => Set<Organization>();
  public DbSet<OrganizationMembership> OrganizationMemberships => Set<OrganizationMembership>();
  public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(OpenCaptiveDbContext).Assembly);
  }

  public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
  {
    var now = DateTimeOffset.UtcNow;

    foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
    {
      switch (entry.State)
      {
        case EntityState.Added:
          {
            entry.Property(e => e.CreatedAt).CurrentValue = now;
            entry.Property(e => e.UpdatedAt).CurrentValue = now;
            break;
          }
        case EntityState.Modified:
          {
            entry.Property(e => e.UpdatedAt).CurrentValue = now;
            break;
          }
      }
    }

    return base.SaveChangesAsync(cancellationToken);
  }
}
