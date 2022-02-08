using System;
using System.Collections.Generic;

#nullable disable

namespace IssueProject.Entity
{
    public  class ManagerDepartment
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int DepartmentId { get; set; }

        public virtual Department Department { get; set; }
        public virtual User User { get; set; }
        public virtual List<User> Users { get; set; }
    }
}
