using Core.Models;
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
    public class JobConfiguration : IEntityTypeConfiguration<Job>
    {
        public void Configure(EntityTypeBuilder<Job> entity)
        {
            entity.HasKey(e => e.Id).HasName("PK__Job__3214EC2704CFD28F");

            entity.ToTable("Job");

            entity.HasIndex(e => e.CategoryId, "IX_Job_CategoryID");

            entity.HasIndex(e => e.EmployerId, "IX_Job_EmployerID");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Description).IsRequired();
            entity.Property(e => e.EmployerId).HasColumnName("EmployerID");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.JobType)
                .IsRequired()
                .HasMaxLength(30);
            entity.Property(e => e.Location).HasMaxLength(200);
            entity.Property(e => e.SalaryRange).HasMaxLength(100);
            entity.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(250);

            entity.HasOne(d => d.Category).WithMany(p => p.Jobs)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Job_Category");

            // In JobConfiguration.cs
            entity.HasOne(d => d.Employer).WithMany(p => p.Jobs)
                .HasForeignKey(d => d.EmployerId)
                .HasConstraintName("FK_Job_Employer");
        }
    }
}
