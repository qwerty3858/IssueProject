using IssueProject.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IssueProject.FluentApiConfigurations
{
    public class IssuePreconditionConfiguration : IEntityTypeConfiguration<IssuePrecondition>
    {
        public void Configure(EntityTypeBuilder<IssuePrecondition> modelBuilder)
        {
            modelBuilder.ToTable("IssuePrecondition");
            modelBuilder.HasKey(e => e.Id);

            modelBuilder.HasIndex(e => e.LineNo, "ix_code");
            modelBuilder.Property(e => e.Explanation)
                .IsRequired()
                .HasMaxLength(512)
                .IsUnicode(false)
                .HasComment("Açıklama");

            modelBuilder.Property(e => e.IssueId).HasComment("Konu Id");

            modelBuilder.Property(e => e.LineNo).HasComment("Satır No");

           

        }
    }
}
