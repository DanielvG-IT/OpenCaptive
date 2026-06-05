using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenCaptive.Domain.Organizations;

namespace OpenCaptive.Infrastructure.Persistence.Configurations;

public sealed class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
{
  public void Configure(EntityTypeBuilder<Organization> builder)
  {
    builder.ToTable("organizations");

    builder.HasKey(x => x.Id);

    builder.Property(x => x.Name)
      .IsRequired()
      .HasMaxLength(200);

    builder.Property(x => x.Slug)
      .IsRequired()
      .HasMaxLength(100);

    builder.HasIndex(x => x.Slug)
      .IsUnique();
  }
}
