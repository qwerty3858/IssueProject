using IssueProject.FluentApiConfigurations;
using Microsoft.EntityFrameworkCore;

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
        public virtual DbSet<IssueSubTitle> IssueSubTitles { get; set; }
        public virtual DbSet<IssueTitle> IssueTitles { get; set; }
        public virtual DbSet<ManagerDepartment> ManagerDepartments { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //FluentApiCompany için

            modelBuilder.ApplyConfiguration(new DepartmentConfiguration());
            modelBuilder.ApplyConfiguration(new IssueActivityConfiguration());
            modelBuilder.ApplyConfiguration(new IssueActivityDetailConfiguration());
            modelBuilder.ApplyConfiguration(new IssueAttachmentConfiguraton());
            modelBuilder.ApplyConfiguration(new IssueConfiguration());
            modelBuilder.ApplyConfiguration(new IssueConfirmConfiguration());
            modelBuilder.ApplyConfiguration(new IssueNoteConfiguration());
            modelBuilder.ApplyConfiguration(new IssuePreconditionConfiguration());
            modelBuilder.ApplyConfiguration(new IssueRelevantDepartmantConfiguration());
            modelBuilder.ApplyConfiguration(new IssueRoleConfiguration());
            modelBuilder.ApplyConfiguration(new RoleConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new UserTokenConfiguration());
            modelBuilder.ApplyConfiguration(new IssueTitleConfiguration());
            modelBuilder.ApplyConfiguration(new IssueSubTitleConfiguration());
            modelBuilder.ApplyConfiguration(new ManagerDepartmentConfiguration());

        }
    }
}
