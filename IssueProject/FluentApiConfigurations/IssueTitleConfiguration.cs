using IssueProject.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IssueProject.FluentApiConfigurations
{
    public class IssueTitleConfiguration : IEntityTypeConfiguration<IssueTitle>
    {
        public void Configure(EntityTypeBuilder<IssueTitle> entity)
        {
            
                entity.ToTable("IssueTitle");

                entity.Property(e => e.Subject)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.HasOne(d => d.Department)
                            .WithMany(p => p.IssueTitles)
                            .HasForeignKey(d => d.DepartmentId)
                            .OnDelete(DeleteBehavior.ClientSetNull)
                            .HasConstraintName("FK_IssueTitle_Department");

                //entity.HasOne(d => d.Issue)
                //            .WithMany(p => p.IssueTitles)
                //            .HasForeignKey(d => d.IssueId)
                //            .OnDelete(DeleteBehavior.ClientSetNull)
                //            .HasConstraintName("FK_IssueTitle_Issue");
            
        }
            
    }
}
