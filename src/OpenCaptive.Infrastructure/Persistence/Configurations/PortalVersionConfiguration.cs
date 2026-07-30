using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenCaptive.Domain.Portals;

namespace OpenCaptive.Infrastructure.Persistence.Configurations;

public sealed class PortalVersionConfiguration : IEntityTypeConfiguration<PortalVersion>
{
  public void Configure(EntityTypeBuilder<PortalVersion> builder)
  {
    builder.ToTable("portal_versions");

    builder.HasKey(x => x.Id);

    builder.Property(x => x.Version);

    builder.Property(x => x.Content)
      .HasColumnType("text");

    builder.Property(x => x.IsPublished);

    builder.HasIndex(e => new { e.PortalId, e.Version })
      .IsUnique();

    builder.HasOne<Portal>()
      .WithMany()
      .HasForeignKey(x => x.PortalId)
      .OnDelete(DeleteBehavior.Cascade);
  }
}
