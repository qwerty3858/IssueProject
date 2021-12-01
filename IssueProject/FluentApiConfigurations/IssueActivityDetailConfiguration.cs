using IssueProject.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IssueProject.FluentApiConfigurations
{
    public class IssueActivityDetailConfiguration : IEntityTypeConfiguration<IssueActivitiyDetail>
    {
        public void Configure(EntityTypeBuilder<IssueActivitiyDetail> modelBuilder)
        {

            modelBuilder.ToTable("IssueActivitiyDetail");
            modelBuilder.Property(e => e.Definition).IsRequired().HasMaxLength(2048).IsUnicode(false).HasComment("Açıklama");
            modelBuilder.Property(e => e.Explanation).HasMaxLength(2048).IsUnicode(false).HasComment("Açıklama");
            modelBuilder.Property(e => e.IssueActivityId).HasComment("Konu Id");
            modelBuilder.Property(e => e.LineNo).HasComment("Sıra No");
            modelBuilder.Property(e => e.Medium).HasComment("Ortam (Excel, Mail vb.)");
            modelBuilder.Property(e => e.RoleId).HasComment("Rol");
            modelBuilder.Property(e => e.ParentId);
            modelBuilder.HasOne(d => d.IssueActivity).WithMany(p => p.IssueActivitiyDetails).HasForeignKey(d => d.IssueActivityId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_IssueActivitiy_IssueActivitiyDetail_");
            
        }
    }
}
