using IssueProject.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IssueProject.FluentApiConfigurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> modelBuilder)
        {

            modelBuilder.ToTable("Role");

            modelBuilder.Property(e => e.Id).ValueGeneratedOnAdd();

            modelBuilder.Property(e => e.Definition)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);

        }
    }
}
