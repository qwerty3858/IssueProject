using System;
using System.Collections.Generic;

#nullable disable

namespace IssueProject.Entity
{
    public partial class Role
    {
        public Role()
        {
            IssueRoles = new HashSet<IssueRole>();
            Users = new HashSet<User>();
        }

        public byte Id { get; set; }
        public string Definition { get; set; }

        public virtual ICollection<IssueRole> IssueRoles { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
