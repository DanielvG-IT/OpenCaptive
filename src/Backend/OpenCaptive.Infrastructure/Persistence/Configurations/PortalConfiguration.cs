using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenCaptive.Domain.Networks;
using OpenCaptive.Domain.Portals;

namespace OpenCaptive.Infrastructure.Persistence.Configurations;

public sealed class PortalConfiguration : IEntityTypeConfiguration<Portal>
{
  public void Configure(EntityTypeBuilder<Portal> builder)
  {
    builder.ToTable("portals");

    builder.HasKey(x => x.Id);

    builder.Property(x => x.Name)
      .IsRequired()
      .HasMaxLength(200);

    builder.Property(x => x.IsPublished);

    builder.Property(x => x.PublishedVersionId);

    builder.HasOne<PortalVersion>()
      .WithOne()
      .HasForeignKey<Portal>(x => x.PublishedVersionId)
      .IsRequired(false)
      .OnDelete(DeleteBehavior.NoAction);

    builder.HasIndex(x => x.NetworkId)
      .IsUnique();

    builder.HasOne<Network>()
        .WithOne()
        .HasForeignKey<Portal>(x => x.NetworkId)
        .OnDelete(DeleteBehavior.Cascade);
  }
}
