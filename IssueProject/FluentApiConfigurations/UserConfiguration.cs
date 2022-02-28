using IssueProject.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IssueProject.FluentApiConfigurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> modelBuilder)
        {

            modelBuilder.ToTable("User");

            modelBuilder.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasComment("Kullanıcı Sicil No");

            modelBuilder.Property(e => e.DepartmentId).HasComment("Departman");

            modelBuilder.Property(e => e.EmailAddress)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);

            modelBuilder.Property(e => e.FullName)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasComment("Tam Adı");

            modelBuilder.Property(e => e.Password)
                .IsRequired()
                .HasMaxLength(32)
                .IsUnicode(false);

            modelBuilder.Property(e => e.RoleId).HasComment("Görevi");

            modelBuilder.HasOne(d => d.Department)
                .WithMany(p => p.Users)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Department_User_");

            modelBuilder.HasOne(d => d.Role)
                .WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Role_User_");

            modelBuilder.HasOne(d => d.Manager)
                .WithMany(p => p.Users)
                .HasForeignKey(d => d.ManagerId)
                .HasConstraintName("FK_User_ManagerDepartment");
        }
    }
}
