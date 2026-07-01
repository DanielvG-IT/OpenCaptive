using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OpenCaptive.Domain.Auth;
using OpenCaptive.Domain.Common;
using OpenCaptive.Domain.Guests;
using OpenCaptive.Domain.Integrations;
using OpenCaptive.Domain.Networks;
using OpenCaptive.Domain.Organizations;
using OpenCaptive.Domain.Portals;
using OpenCaptive.Domain.Sites;
using OpenCaptive.Infrastructure.Auth;

namespace OpenCaptive.Infrastructure.Persistence;

public sealed class OpenCaptiveDbContext(DbContextOptions<OpenCaptiveDbContext> options) : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>(options)
{
  public DbSet<Organization> Organizations => Set<Organization>();
  public DbSet<OrganizationMembership> OrganizationMemberships => Set<OrganizationMembership>();
  public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
  public DbSet<Site> Sites => Set<Site>();
  public DbSet<SiteIntegration> SiteIntegrations => Set<SiteIntegration>();
  public DbSet<Network> Networks => Set<Network>();
  public DbSet<GuestSession> GuestSessions => Set<GuestSession>();
  public DbSet<Portal> Portals => Set<Portal>();
  public DbSet<PortalVersion> PortalVersions => Set<PortalVersion>();

  protected override void OnModelCreating(ModelBuilder builder)
  {
    base.OnModelCreating(builder);

    builder.Entity<ApplicationUser>().Property(x => x.FirstName)
        .IsRequired()
        .HasMaxLength(100);

    builder.Entity<ApplicationUser>().Property(x => x.LastName)
        .IsRequired()
        .HasMaxLength(100);

    builder.ApplyConfigurationsFromAssembly(typeof(OpenCaptiveDbContext).Assembly);
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
