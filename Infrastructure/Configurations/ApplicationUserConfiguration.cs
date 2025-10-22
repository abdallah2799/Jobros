// 1. FIX THE USING STATEMENTS
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

// 2. FIX THE NAMESPACE
namespace Jobros.Infrastructure.Configurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.ToTable("Users"); // Identity default table

        builder.HasDiscriminator<string>("Role")
            .HasValue<Admin>("Admin")
            .HasValue<Employer>("Employer")
            .HasValue<JobSeeker>("JobSeeker");

        builder.Property(u => u.FullName)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(u => u.CreatedAt)
            .HasDefaultValueSql("(sysdatetime())");

        builder.Property(u => u.IsActive)
            .HasDefaultValue(true);
    }
}
