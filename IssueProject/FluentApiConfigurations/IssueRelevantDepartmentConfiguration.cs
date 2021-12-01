using IssueProject.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IssueProject.FluentApiConfigurations
{
    public class IssueRelevantDepartmantConfiguration : IEntityTypeConfiguration<IssueRelevantDepartmant>
    {
        public void Configure(EntityTypeBuilder<IssueRelevantDepartmant> modelBuilder)
        {

            modelBuilder.ToTable("IssueRelevantDepartmant");

            modelBuilder.HasOne(d => d.Department)
                .WithMany(p => p.IssueRelevantDepartmants)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Department_IssueRelevantDepartmant_");

            modelBuilder.HasOne(d => d.Issue)
                .WithMany(p => p.IssueRelevantDepartmants)
                .HasForeignKey(d => d.IssueId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Issue_IssueRelevantDepartmant_");

        }
    }
}
