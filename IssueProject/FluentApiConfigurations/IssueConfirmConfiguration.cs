using IssueProject.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IssueProject.FluentApiConfigurations
{
    public class IssueConfirmConfiguration : IEntityTypeConfiguration<IssueConfirm>
    {
        public void Configure(EntityTypeBuilder<IssueConfirm> modelBuilder)
        {

            modelBuilder.ToTable("IssueConfirm");

            modelBuilder.Property(e => e.CreateTime).HasColumnType("datetime"); 

            modelBuilder.Property(e => e.Description)
               
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasComment("Red sebebi");

            modelBuilder.Property(e => e.MailTime).HasColumnType("datetime");

            modelBuilder.Property(e => e.Status).HasComment("0) Mail Gönderilmedi\r\n1) Mail Gönderildi Beklemede\r\n2) Onaylandı\r\n3) Reddedildi");

            modelBuilder.Property(e => e.SubmitTime).HasColumnType("datetime");
            modelBuilder.Property(e => e.IsConfirm); 
            modelBuilder.Property(e => e.IsCreated); 

            modelBuilder.HasOne(d => d.Issue)
                .WithMany(p => p.IssueConfirms)
                .HasForeignKey(d => d.IssueId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Issue_IssueHistory_");

        }
    }
}
