using IssueProject.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IssueProject.FluentApiConfigurations
{
    public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> modelBuilder)
        {

            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1254_CI_AS");
            modelBuilder.ToTable("Department");
            modelBuilder.Property(e => e.Definition).HasMaxLength(50).IsUnicode(false);
            
        }
    }
}