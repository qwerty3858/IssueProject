using IssueProject.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IssueProject.FluentApiConfigurations
{
    public class UserTokenConfiguration : IEntityTypeConfiguration<UserToken>
    {
        public void Configure(EntityTypeBuilder<UserToken> modelBuilder)
        {

            modelBuilder.ToTable("UserToken");
            modelBuilder.Property(e => e.Id).HasMaxLength(100);
            modelBuilder.Property(e => e.ClientId).HasMaxLength(50);
            modelBuilder.Property(e => e.ExpiresUtc).HasColumnType("datetime");
            modelBuilder.Property(e => e.IssuedUtc).HasColumnType("datetime");
            modelBuilder.Property(e => e.Subject).HasMaxLength(50);

        }
    }
}
