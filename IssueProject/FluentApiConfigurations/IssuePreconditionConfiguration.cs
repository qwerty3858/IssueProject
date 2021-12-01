using IssueProject.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IssueProject.FluentApiConfigurations
{
    public class IssuePreconditionConfiguration : IEntityTypeConfiguration<IssuePrecondition>
    {
        public void Configure(EntityTypeBuilder<IssuePrecondition> modelBuilder)
        {
            modelBuilder.ToTable("IssuePrecondition");

            modelBuilder.Property(e => e.Explanation)
                .IsRequired()
                .HasMaxLength(512)
                .IsUnicode(false)
                .HasComment("Açıklama");

            modelBuilder.Property(e => e.IssueId).HasComment("Konu Id");

            modelBuilder.Property(e => e.LineNo).HasComment("Satır No");

            modelBuilder.HasOne(d => d.Issue)
                .WithMany(p => p.IssuePreconditions)
                .HasForeignKey(d => d.IssueId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Issue_IssuePrecondition_");

        }
    }
}
