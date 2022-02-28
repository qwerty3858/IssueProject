using IssueProject.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IssueProject.FluentApiConfigurations
{
    public class IssueActivityDetailConfiguration : IEntityTypeConfiguration<IssueActivitiyDetail>
    {
        public void Configure(EntityTypeBuilder<IssueActivitiyDetail> modelBuilder)
        {

            modelBuilder.ToTable("IssueActivitiyDetail");
            modelBuilder.HasKey(a => a.Id);

            modelBuilder.Property(e => e.Definition).IsRequired().HasMaxLength(2048).IsUnicode(false).HasComment("Açıklama");
            modelBuilder.Property(e => e.Explanation).HasMaxLength(2048).IsUnicode(false).HasComment("Açıklama");
            modelBuilder.Property(e => e.IssueActivitiyId).HasComment("Konu Id");
            modelBuilder.Property(e => e.LineNo).HasComment("Sıra No");
            modelBuilder.Property(e => e.Medium).HasComment("Ortam (Excel, Mail vb.)");
            modelBuilder.Property(e => e.RoleId).HasComment("Rol");
            modelBuilder.Property(e => e.ParentId);
            /*
            modelBuilder.HasOne(z => z.IssueActivitiy)
                .WithMany(z => z.IssueActivitiyDetails)
                .HasForeignKey(d => d.IssueActivitiyId)
                .HasConstraintName("FK_IssueActivitiyDetails_IssueActivities_IssueActivitiyId");*/
            /*
            modelBuilder
                .HasMany(z => z.IssueActivitiyDetails)
                .WithOne()
                .HasForeignKey(z => z.ParentId)
                .HasConstraintName("FK_IssueActivitiyDetails_IssueActivitiyDetails_ParentId");*/
            
            modelBuilder.HasOne(z => z.Parent)
                .WithMany(detail => detail.IssueActivitiyDetails)
                .HasForeignKey(d => d.ParentId)
                .HasConstraintName("FK_IssueActivitiyDetails_IssueActivitiyDetails_ParentId");
        }
    }
}
