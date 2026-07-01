using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenCaptive.Domain.Organizations;
using OpenCaptive.Domain.Sites;

namespace OpenCaptive.Infrastructure.Persistence.Configurations;

public sealed class SiteConfiguration : IEntityTypeConfiguration<Site>
{
  public void Configure(EntityTypeBuilder<Site> builder)
  {
    builder.ToTable("sites");

    builder.HasKey(x => x.Id);

    builder.Property(x => x.Name)
      .IsRequired()
      .HasMaxLength(200);

    builder.Property(x => x.Slug)
      .IsRequired()
      .HasMaxLength(100);

    builder.Property(x => x.Description)
      .HasColumnType("text");

    builder.Property(x => x.TimeZone)
      .IsRequired()
      .HasMaxLength(64);

    builder.Property(x => x.IsEnabled);

    builder.HasIndex(e => new { e.OrganizationId, e.Slug })
      .IsUnique();

    builder.HasOne<Organization>()
      .WithMany()
      .HasForeignKey(x => x.OrganizationId)
      .OnDelete(DeleteBehavior.Cascade);
  }
}
