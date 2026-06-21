using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenCaptive.Domain.Organizations;
using OpenCaptive.Infrastructure.Identity;

namespace OpenCaptive.Infrastructure.Persistence.Configurations;

public sealed class OrganizationMembershipConfiguration : IEntityTypeConfiguration<OrganizationMembership>
{
  public void Configure(EntityTypeBuilder<OrganizationMembership> builder)
  {
    builder.ToTable("organization_memberships");

    builder.HasKey(x => x.Id);

    // Single-org for now: relax to (UserId, OrganizationId) when multi-org lands.
    builder.HasIndex(x => x.UserId)
      .IsUnique();

    builder.HasIndex(x => x.OrganizationId);

    builder.HasOne<ApplicationUser>()
      .WithMany()
      .HasForeignKey(x => x.UserId)
      .OnDelete(DeleteBehavior.Cascade);

    builder.HasOne<Organization>()
      .WithMany()
      .HasForeignKey(x => x.OrganizationId)
      .OnDelete(DeleteBehavior.Cascade);
  }
}
