using IssueProject.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IssueProject.FluentApiConfigurations
{
    public class IssueAttachmentConfiguraton : IEntityTypeConfiguration<IssueAttachment>
        {
            public void Configure(EntityTypeBuilder<IssueAttachment> modelBuilder)
            {
            modelBuilder.ToTable("IssueAttachment");

            modelBuilder.Property(e => e.FileName)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);

            modelBuilder.Property(e => e.UniqueName)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);

            modelBuilder.HasOne(d => d.Issue)
                .WithMany(p => p.IssueAttachments)
                .HasForeignKey(d => d.IssueId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Issue_IssueAttachment_");

        }
    }
}