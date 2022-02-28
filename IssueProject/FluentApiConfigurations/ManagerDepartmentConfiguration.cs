using IssueProject.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IssueProject.FluentApiConfigurations
{
    public class ManagerDepartmentConfiguration : IEntityTypeConfiguration<ManagerDepartment>
    {
        public void Configure(EntityTypeBuilder<ManagerDepartment> entity)
        {
            entity.ToTable("ManagerDepartment");
            entity.HasOne(d => d.Department)
                  .WithMany(p => p.ManagerDepartments)
                  .HasForeignKey(d => d.DepartmentId)
                  .OnDelete(DeleteBehavior.ClientSetNull)
                  .HasConstraintName("FK_ManagerDepartment_Department");

            entity.HasOne(d => d.User)
                        .WithMany(p => p.ManagerDepartments)
                        .HasForeignKey(d => d.UserId)
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_ManagerDepartment_User");
        }
      
    }
}
