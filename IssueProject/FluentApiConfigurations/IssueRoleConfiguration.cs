using IssueProject.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IssueProject.FluentApiConfigurations
{
    public class IssueRoleConfiguration : IEntityTypeConfiguration<IssueRole>
    {
        public void Configure(EntityTypeBuilder<IssueRole> modelBuilder)
        {

            modelBuilder.ToTable("IssueRole");

            modelBuilder.HasOne(d => d.Issue)
                .WithMany(p => p.IssueRoles)
                .HasForeignKey(d => d.IssueId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Issue_IssueRole_");

            modelBuilder.HasOne(d => d.Role)
                .WithMany(p => p.IssueRoles)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Role_IssueRole_");

        }
    }
}
