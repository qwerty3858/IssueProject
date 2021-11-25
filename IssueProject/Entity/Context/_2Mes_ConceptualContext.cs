using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace IssueProject.Entity.Context
{
    public partial class _2Mes_ConceptualContext : DbContext
    {
        public _2Mes_ConceptualContext()
        {
        }

        public _2Mes_ConceptualContext(DbContextOptions<_2Mes_ConceptualContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<Issue> Issues { get; set; }
        public virtual DbSet<IssueActivitiy> IssueActivitiys { get; set; }
        public virtual DbSet<IssueActivitiyDetail> IssueActivitiyDetails { get; set; }
        public virtual DbSet<IssueAttachment> IssueAttachments { get; set; }
        public virtual DbSet<IssueConfirm> IssueConfirms { get; set; }
        public virtual DbSet<IssueNote> IssueNotes { get; set; }
        public virtual DbSet<IssuePrecondition> IssuePreconditions { get; set; }
        public virtual DbSet<IssueRelevantDepartmant> IssueRelevantDepartmants { get; set; }
        public virtual DbSet<IssueRole> IssueRoles { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserToken> UserTokens { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1254_CI_AS");

            modelBuilder.Entity<Department>(entity =>
            {
                entity.ToTable("Department");

                entity.Property(e => e.Definition)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Issue>(entity =>
            {
                entity.ToTable("Issue");

                entity.Property(e => e.DepartmentId).HasComment("Departman");

                entity.Property(e => e.IssueNo).HasComment("Konu No");

                entity.Property(e => e.Keywords)
                    .IsRequired()
                    .HasMaxLength(512)
                    .IsUnicode(false)
                    .HasComment("Anahtar kelimeler");

                entity.Property(e => e.Status).HasComment("0) Çalışılıyor\r\n1) BIM Onay Bekleme\r\n2) BİM Onay\r\n3) Departman Onay\r\n4) Yazan Departman Amir Onay\r\n5) Kilitli\r\n9) Red/Yapılmayacak");

                entity.Property(e => e.Subtitle)
                    .HasMaxLength(256)
                    .IsUnicode(false)
                    .HasComment("Alt başlık");

                entity.Property(e => e.Summary)
                    .IsRequired()
                    .HasMaxLength(1024)
                    .IsUnicode(false)
                    .HasComment("Kısa açıklama");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(256)
                    .IsUnicode(false)
                    .HasComment("Başlık");

                entity.Property(e => e.UserId).HasComment("Kullanıcı Id");

                entity.Property(e => e.WorkArea).HasComment("Üretim Yeri 550/552");

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.Issues)
                    .HasForeignKey(d => d.DepartmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Department_Issue_");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Issues)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_User_Issue_");
            });

            modelBuilder.Entity<IssueActivitiy>(entity =>
            {
                entity.ToTable("IssueActivitiy");

                entity.Property(e => e.SubActivityNo).HasComment("Sıra No");

                entity.Property(e => e.SubActivityTitle)
                    .IsRequired()
                    .HasMaxLength(256)
                    .IsUnicode(false);

                entity.Property(e => e.Type)
                    .HasDefaultValueSql("((1))")
                    .HasComment("1) Temel Aktivite\r\n2) Alternatif Aktivite\r\n3) İşlem İptal Aktivite");

                entity.HasOne(d => d.Issue)
                    .WithMany(p => p.IssueActivitiys)
                    .HasForeignKey(d => d.IssueId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Issue_IssueActivitiy_Id");
            });

            modelBuilder.Entity<IssueActivitiyDetail>(entity =>
            {
                entity.ToTable("IssueActivitiyDetail");

                entity.Property(e => e.Definition)
                    .IsRequired()
                    .HasMaxLength(2048)
                    .IsUnicode(false)
                    .HasComment("Açıklama");

                entity.Property(e => e.Explanation)
                    .IsRequired()
                    .HasMaxLength(2048)
                    .IsUnicode(false)
                    .HasComment("Açıklama");

                entity.Property(e => e.IssueActivityId).HasComment("Konu Id");

                entity.Property(e => e.LineNo).HasComment("Sıra No");

                entity.Property(e => e.Medium).HasComment("Ortam (Excel, Mail vb.)");

                entity.Property(e => e.RoleId).HasComment("Rol");

                entity.HasOne(d => d.IssueActivity)
                    .WithMany(p => p.IssueActivitiyDetails)
                    .HasForeignKey(d => d.IssueActivityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_IssueActivitiy_IssueActivitiyDetail_");
            });

            modelBuilder.Entity<IssueAttachment>(entity =>
            {
                entity.ToTable("IssueAttachment");

                entity.Property(e => e.FileName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.UniqueName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.Issue)
                    .WithMany(p => p.IssueAttachments)
                    .HasForeignKey(d => d.IssueId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Issue_IssueAttachment_");
            });

            modelBuilder.Entity<IssueConfirm>(entity =>
            {
                entity.ToTable("IssueConfirm");

                entity.Property(e => e.CreateTime).HasColumnType("datetime");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasComment("Red sebebi");

                entity.Property(e => e.MailTime).HasColumnType("datetime");

                entity.Property(e => e.Status).HasComment("0) Mail Gönderilmedi\r\n1) Mail Gönderildi Beklemede\r\n2) Onaylandı\r\n3) Reddedildi");

                entity.Property(e => e.SubmitTime).HasColumnType("datetime");

                entity.HasOne(d => d.Issue)
                    .WithMany(p => p.IssueConfirms)
                    .HasForeignKey(d => d.IssueId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Issue_IssueHistory_");
            });

            modelBuilder.Entity<IssueNote>(entity =>
            {
                entity.ToTable("IssueNote");

                entity.Property(e => e.Explanation)
                    .IsRequired()
                    .HasMaxLength(2048)
                    .IsUnicode(false);

                entity.HasOne(d => d.Issue)
                    .WithMany(p => p.IssueNotes)
                    .HasForeignKey(d => d.IssueId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Issue_IssueNote_");
            });

            modelBuilder.Entity<IssuePrecondition>(entity =>
            {
                entity.ToTable("IssuePrecondition");

                entity.Property(e => e.Explanation)
                    .IsRequired()
                    .HasMaxLength(512)
                    .IsUnicode(false)
                    .HasComment("Açıklama");

                entity.Property(e => e.IssueId).HasComment("Konu Id");

                entity.Property(e => e.LineNo).HasComment("Satır No");

                entity.HasOne(d => d.Issue)
                    .WithMany(p => p.IssuePreconditions)
                    .HasForeignKey(d => d.IssueId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Issue_IssuePrecondition_");
            });

            modelBuilder.Entity<IssueRelevantDepartmant>(entity =>
            {
                entity.ToTable("IssueRelevantDepartmant");

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.IssueRelevantDepartmants)
                    .HasForeignKey(d => d.DepartmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Department_IssueRelevantDepartmant_");

                entity.HasOne(d => d.Issue)
                    .WithMany(p => p.IssueRelevantDepartmants)
                    .HasForeignKey(d => d.IssueId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Issue_IssueRelevantDepartmant_");
            });

            modelBuilder.Entity<IssueRole>(entity =>
            {
                entity.ToTable("IssueRole");

                entity.Property(e => e.Id);

                entity.HasOne(d => d.Issue)
                    .WithMany(p => p.IssueRoles)
                    .HasForeignKey(d => d.IssueId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Issue_IssueRole_");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.IssueRoles)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Role_IssueRole_");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Role");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Definition)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");

                entity.Property(e => e.Id).HasComment("Kullanıcı Sicil No");

                entity.Property(e => e.DepartmentId).HasComment("Departman");

                entity.Property(e => e.EmailAddress)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasComment("Tam Adı");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(32)
                    .IsUnicode(false);

                entity.Property(e => e.RoleId).HasComment("Görevi");

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.DepartmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Department_User_");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Role_User_");
            });
            modelBuilder.Entity<UserToken>(entity =>
            {
                
                entity.ToTable("UserToken");

                entity.Property(e => e.Id).HasMaxLength(100);

                entity.Property(e => e.ClientId).HasMaxLength(50);

                entity.Property(e => e.ExpiresUtc).HasColumnType("datetime");

                entity.Property(e => e.IssuedUtc).HasColumnType("datetime");

                 

                entity.Property(e => e.Subject).HasMaxLength(50);
            });
            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
