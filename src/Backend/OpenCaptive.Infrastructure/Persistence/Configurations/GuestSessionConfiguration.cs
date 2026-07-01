using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenCaptive.Domain.Guests;
using OpenCaptive.Domain.Networks;
using OpenCaptive.Domain.Sites;

namespace OpenCaptive.Infrastructure.Persistence.Configurations;

public sealed class GuestSessionConfiguration : IEntityTypeConfiguration<GuestSession>
{
  public void Configure(EntityTypeBuilder<GuestSession> builder)
  {
    builder.ToTable("guest_sessions");

    builder.HasKey(x => x.Id);

    builder.Property(x => x.ClientMacAddress)
      .IsRequired()
      .HasMaxLength(17);

    builder.Property(x => x.ClientIpAddress)
      .HasMaxLength(45); // Supports IPv4 and IPv6

    builder.Property(x => x.StartedAt);

    builder.Property(x => x.AuthenticatedAt);

    builder.Property(x => x.EndedAt);

    builder.Property(x => x.IsAuthenticated);

    builder.HasIndex(x => x.SiteId);

    builder.HasIndex(x => x.NetworkId);

    builder.HasIndex(x => x.StartedAt);

    builder.HasIndex(x => x.ClientMacAddress);

    builder.HasIndex(e => new { e.SiteId, e.StartedAt });

    builder.HasOne<Site>()
      .WithMany()
      .HasForeignKey(x => x.SiteId)
      .OnDelete(DeleteBehavior.Cascade);

    builder.HasOne<Network>()
      .WithMany()
      .HasForeignKey(x => x.NetworkId)
      .OnDelete(DeleteBehavior.Cascade);
  }
}
