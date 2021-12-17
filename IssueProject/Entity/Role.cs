using System.Collections.Generic;

#nullable disable

namespace IssueProject.Entity
{
    public partial class Role
    {
       

        public byte Id { get; set; }
        public string Definition { get; set; }

        public virtual List<IssueRole> IssueRoles { get; set; }
        public virtual List<User> Users { get; set; }
    }
}
