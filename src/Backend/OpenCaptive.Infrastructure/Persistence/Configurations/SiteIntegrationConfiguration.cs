using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenCaptive.Domain.Integrations;
using OpenCaptive.Domain.Sites;

namespace OpenCaptive.Infrastructure.Persistence.Configurations;

public sealed class SiteIntegrationConfiguration : IEntityTypeConfiguration<SiteIntegration>
{
  public void Configure(EntityTypeBuilder<SiteIntegration> builder)
  {
    builder.ToTable("site_integrations");

    builder.HasKey(x => x.Id);

    builder.Property(x => x.Provider)
      .IsRequired()
      .HasMaxLength(100);

    builder.Property(x => x.DisplayName)
      .IsRequired()
      .HasMaxLength(200);

    builder.Property(x => x.LastError)
      .HasColumnType("text");

    builder.Property(x => x.IsEnabled);

    builder.Property(x => x.LastSuccessfulConnectionAt);

    builder.Property(x => x.LastCapabilityRefreshAt);

    builder.HasIndex(x => x.SiteId)
      .IsUnique();

    builder.HasOne<Site>()
      .WithMany()
      .HasForeignKey(x => x.SiteId)
      .OnDelete(DeleteBehavior.Cascade);
  }
}
