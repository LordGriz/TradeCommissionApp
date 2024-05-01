
using Domain.Objects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class FeesConfiguration : IEntityTypeConfiguration<Fee>
{
    public void Configure(EntityTypeBuilder<Fee> builder)
    {
        builder.ToTable("Fees");
        builder.HasKey(f => f.Id);

        builder.Property(f => f.SecurityType)
            .HasMaxLength(100);

        builder.Property(f => f.Description)
            .HasMaxLength(100);
    }
}