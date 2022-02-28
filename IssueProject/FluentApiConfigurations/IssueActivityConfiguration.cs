using IssueProject.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IssueProject.FluentApiConfigurations
{
    public class IssueActivityConfiguration : IEntityTypeConfiguration<IssueActivitiy>
    {
        public void Configure(EntityTypeBuilder<IssueActivitiy> modelBuilder)
        {

            modelBuilder.ToTable("IssueActivitiy");
            modelBuilder.HasKey(a => a.Id);
             
            modelBuilder.Property(e => e.SubActivityNo).HasComment("Sıra No");
            modelBuilder.Property(e => e.SubActivityTitle).HasMaxLength(256).IsUnicode(false);
            modelBuilder.Property(e => e.Type).HasDefaultValueSql("((1))").HasComment("1) Temel Aktivite\r\n2) Alternatif Aktivite\r\n3) İşlem İptal Aktivite");


            modelBuilder.HasMany(z => z.IssueActivitiyDetails)
               .WithOne(detail => detail.IssueActivitiy)
               .HasForeignKey(d => d.IssueActivitiyId)
               .HasConstraintName("FK_IssueActivitiyDetails_IssueActivities_IssueActivitiyId");
        }
    }
}
