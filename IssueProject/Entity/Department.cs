using System.Collections.Generic;

namespace IssueProject.Entity
{
    public partial class Department
    {
        public int Id { get; set; }
        public string Definition { get; set; }
        public bool IsITDepartment { get; set; }
        public virtual List<IssueRelevantDepartmant> IssueRelevantDepartmants { get; set; }
        public virtual List<Issue> Issues { get; set; }
        public virtual List<User> Users { get; set; }
        public virtual List<IssueTitle> IssueTitles { get; set; }
        public virtual List<ManagerDepartment> ManagerDepartments { get; set; }
    }
}