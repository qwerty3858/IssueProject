using IssueProject.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IssueProject.FluentApiConfigurations
{
    public class IssueSubTitleConfiguration : IEntityTypeConfiguration<IssueSubTitle>
    {
        public void Configure(EntityTypeBuilder<IssueSubTitle> entity)
        {
            entity.ToTable("IssueSubTitle");

            //entity.Property(e => e.Id).ValueGeneratedNever();

            entity.Property(e => e.SubTitle)
                        .IsRequired()
                        .HasMaxLength(250);

            entity.HasOne(d => d.Title)
                        .WithMany(p => p.IssueSubTitles)
                        .HasForeignKey(d => d.TitleId)
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_IssueSubTitle_IssueTitle");
        }

        
    }
}
