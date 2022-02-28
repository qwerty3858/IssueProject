using IssueProject.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IssueProject.FluentApiConfigurations
{
    public class IssueNoteConfiguration : IEntityTypeConfiguration<IssueNote>
    {
        public void Configure(EntityTypeBuilder<IssueNote> modelBuilder)
        {

            modelBuilder.ToTable("IssueNote");

            modelBuilder.Property(e => e.Explanation)
                .IsRequired()
                .HasMaxLength(2048)
                .IsUnicode(false);

            //modelBuilder.HasOne(d => d.Issue)
            //    .WithMany(p => p.IssueNotes)
            //    .HasForeignKey(d => d.IssueId)
            //    .OnDelete(DeleteBehavior.ClientSetNull)
            //    .HasConstraintName("FK_Issue_IssueNote_");

        }
    }
}
