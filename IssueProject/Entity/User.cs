using System.Collections.Generic;

#nullable disable

namespace IssueProject.Entity
{
    public partial class User
    {
         

        public int Id { get; set; }
        public int DepartmentId { get; set; }
        public int ManagerId { get; set; }
        public byte RoleId { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }
        public string EmailAddress { get; set; }
        public bool Deleted { get; set; }
        public bool IsManager { get; set; }
        public bool IsKeyUser { get; set; }
        public bool IsCreated { get; set; }
        public bool IsVisible { get; set; }
        public virtual Department Department { get; set; }
        public virtual Role Role { get; set; }
        public virtual List<Issue> Issues { get; set; }
        public virtual ManagerDepartment Manager { get; set; }
        public virtual List<ManagerDepartment> ManagerDepartments { get; set; }
    }
}
