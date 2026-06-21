using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenCaptive.Domain.Auth;
using OpenCaptive.Infrastructure.Identity;

namespace OpenCaptive.Infrastructure.Persistence.Configurations;

public sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
  public void Configure(EntityTypeBuilder<RefreshToken> builder)
  {
    builder.ToTable("refresh_tokens");

    builder.HasKey(x => x.Id);

    // SHA-256 hex digest.
    builder.Property(x => x.TokenHash)
      .IsRequired()
      .HasMaxLength(64);

    builder.HasIndex(x => x.TokenHash)
      .IsUnique();

    // Family-wide revocation on reuse detection.
    builder.HasIndex(x => new { x.UserId, x.FamilyId });

    builder.HasOne<ApplicationUser>()
      .WithMany()
      .HasForeignKey(x => x.UserId)
      .OnDelete(DeleteBehavior.Cascade);
  }
}
