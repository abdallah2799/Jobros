using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Configurations
{
    public class ApplicationConfiguration : IEntityTypeConfiguration<Application>
    {
        public void Configure(EntityTypeBuilder<Application> entity)
        {
            entity.HasKey(e => e.Id).HasName("PK__Applicat__3214EC279A6D3465");

            entity.ToTable("Application");

            entity.HasIndex(e => e.JobId, "IX_Application_JobID");

            entity.HasIndex(e => e.JobSeekerId, "IX_Application_JobSeekerID");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.AppliedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.JobId).HasColumnName("JobID");
            entity.Property(e => e.JobSeekerId).HasColumnName("JobSeekerID");
            entity.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(20)
                .HasDefaultValue("Pending");

            entity.HasOne(d => d.Job).WithMany(p => p.Applications)
                .HasForeignKey(d => d.JobId)
                .HasConstraintName("FK_Application_Job");

            // In ApplicationConfiguration.cs
            entity.HasOne(d => d.JobSeeker).WithMany(p => p.Applications)
                .HasForeignKey(d => d.JobSeekerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Application_JobSeeker");
        }
    }
}
