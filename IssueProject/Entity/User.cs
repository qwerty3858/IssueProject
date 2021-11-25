using System;
using System.Collections.Generic;

#nullable disable

namespace IssueProject.Entity
{
    public partial class User
    {
        public User()
        {
            Issues = new HashSet<Issue>();
        }

        public int Id { get; set; }
        public int DepartmentId { get; set; }
        public byte RoleId { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public string EmailAddress { get; set; }
        public bool Deleted { get; set; }

        public virtual Department Department { get; set; }
        public virtual Role Role { get; set; }
        public virtual ICollection<Issue> Issues { get; set; }
    }
}
