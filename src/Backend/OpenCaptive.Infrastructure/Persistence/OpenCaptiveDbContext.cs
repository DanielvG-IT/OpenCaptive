using Microsoft.EntityFrameworkCore;
using OpenCaptive.Domain.Common;
using OpenCaptive.Domain.Organizations;

namespace OpenCaptive.Infrastructure.Persistence;

public sealed class OpenCaptiveDbContext : DbContext
{
  public OpenCaptiveDbContext(DbContextOptions<OpenCaptiveDbContext> options)
    : base(options)
  {
  }

  public DbSet<Organization> Organizations => Set<Organization>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
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
