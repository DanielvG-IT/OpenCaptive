using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenCaptive.Domain.Integrations;
using OpenCaptive.Domain.Networks;
using OpenCaptive.Domain.Sites;

namespace OpenCaptive.Infrastructure.Persistence.Configurations;

public sealed class NetworkConfiguration : IEntityTypeConfiguration<Network>
{
  public void Configure(EntityTypeBuilder<Network> builder)
  {
    builder.ToTable("networks");

    builder.HasKey(x => x.Id);

    builder.Property(x => x.Name)
      .IsRequired()
      .HasMaxLength(200);

    builder.Property(x => x.ProviderNetworkId)
      .IsRequired()
      .HasMaxLength(200);

    builder.Property(x => x.IsGuestNetwork);

    builder.Property(x => x.IsEnabled);

    builder.HasIndex(x => x.SiteId);

    builder.HasIndex(x => x.SiteIntegrationId);

    builder.HasIndex(e => new { e.SiteId, e.ProviderNetworkId })
      .IsUnique();

    builder.HasOne<Site>()
      .WithMany()
      .HasForeignKey(x => x.SiteId)
      .OnDelete(DeleteBehavior.Cascade);

    builder.HasOne<SiteIntegration>()
      .WithMany()
      .HasForeignKey(x => x.SiteIntegrationId)
      .OnDelete(DeleteBehavior.Cascade);
  }
}
