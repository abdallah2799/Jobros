// 1. FIX THE USING STATEMENTS
using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

// 2. FIX THE NAMESPACE
namespace Jobros.Infrastructure.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder) // Renamed 'entity' to 'builder' for clarity
    {
        // 3. MAP ALL CLASSES TO THE "User" TABLE
        builder.ToTable("User");

        // 4. THIS IS THE MOST IMPORTANT PART: CONFIGURE THE INHERITANCE (TPH)
        //    This replaces the old entity.Property(e => e.Role)... configuration
        builder.HasDiscriminator<string>("Role") // The discriminator column is "Role" and it's a string
            .HasValue<Admin>("Admin")           // If Role == "Admin", create an Admin object
            .HasValue<Employer>("Employer")     // If Role == "Employer", create an Employer object
            .HasValue<JobSeeker>("JobSeeker");   // If Role == "JobSeeker", create a JobSeeker object

        // 5. CONFIGURE PROPERTIES COMMON TO ALL USERS (FROM THE BASE 'User' CLASS)
        builder.HasKey(e => e.Id);

        builder.HasIndex(e => e.Email).IsUnique();

        builder.Property(e => e.Id).HasColumnName("ID");

        builder.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");

        builder.Property(e => e.Email)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.FullName)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(e => e.IsActive).HasDefaultValue(true);

        builder.Property(e => e.PasswordHash).IsRequired();

        // Note: Properties unique to Employer (e.g., CompanyName) or JobSeeker (e.g., Bio)
        // do not need to be configured here. EF Core will map them by convention.
    }
}