using IssueProject.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IssueProject.FluentApiConfigurations
{
    public class IssueActivityConfiguration : IEntityTypeConfiguration<IssueActivitiy>
    {
        public void Configure(EntityTypeBuilder<IssueActivitiy> modelBuilder)
        {

            modelBuilder.ToTable("IssueActivitiy");
            modelBuilder.Property(e => e.SubActivityNo).HasComment("Sıra No");
            modelBuilder.Property(e => e.SubActivityTitle).HasMaxLength(256).IsUnicode(false);
            modelBuilder.Property(e => e.Type).HasDefaultValueSql("((1))").HasComment("1) Temel Aktivite\r\n2) Alternatif Aktivite\r\n3) İşlem İptal Aktivite");
            modelBuilder.HasOne(d => d.Issue).WithMany(p => p.IssueActivitiys).HasForeignKey(d => d.IssueId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_Issue_IssueActivitiy_Id");

        }
    }
}
