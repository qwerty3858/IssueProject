using IssueProject.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IssueProject.FluentApiConfigurations
{
    public class IssueConfiguration : IEntityTypeConfiguration<Issue>
    {
        public void Configure(EntityTypeBuilder<Issue> modelBuilder)
        {

            modelBuilder.ToTable("Issue");
            modelBuilder.HasKey(e => e.Id);

            modelBuilder.Property(e => e.DepartmentId).HasComment("Departman");

            modelBuilder.Property(e => e.IssueNo).HasComment("Konu No");

            modelBuilder.Property(e => e.Keywords).IsRequired().HasMaxLength(512).IsUnicode(false).HasComment("Anahtar kelimeler");

            modelBuilder.Property(e => e.Status).HasComment("0) Çalışılıyor\r\n1) BIM Onay Bekleme\r\n2) BİM Onay\r\n3) Departman Onay\r\n4) Yazan Departman Amir Onay\r\n5) Kilitli\r\n9) Red/Yapılmayacak");


            modelBuilder.Property(e => e.Summary).IsRequired().HasMaxLength(1024).IsUnicode(false).HasComment("Kısa açıklama");


            modelBuilder.Property(e => e.UserId).HasComment("Kullanıcı Id");

            //modelBuilder.Property(e => e.WorkArea).HasMaxLength(50).IsUnicode(false).HasComment("Üretim Yeri 550/552");

            modelBuilder.HasOne(d => d.Department).WithMany(p => p.Issues).HasForeignKey(d => d.DepartmentId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_Department_Issue_");

            modelBuilder.HasOne(d => d.User).WithMany(p => p.Issues).HasForeignKey(d => d.UserId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_User_Issue_");

            // modelBuilder.HasOne(d => d.Issue)
            //.WithMany(p => p.IssuePreconditions)
            //.HasForeignKey(d => d.IssueId)
            //.OnDelete(DeleteBehavior.ClientSetNull)
            //.HasConstraintName("FK_Issue_IssuePrecondition_");

        }
    }
}
