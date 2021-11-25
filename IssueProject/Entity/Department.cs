using System;
using System.Collections.Generic;

#nullable disable

namespace IssueProject.Entity
{
    public partial class Department
    {
        public Department()
        {
            IssueRelevantDepartmants = new HashSet<IssueRelevantDepartmant>();
            Issues = new HashSet<Issue>();
            Users = new HashSet<User>();
        }

        public int Id { get; set; }
        public string Definition { get; set; }

        public virtual ICollection<IssueRelevantDepartmant> IssueRelevantDepartmants { get; set; }
        public virtual ICollection<Issue> Issues { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}