using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IssueProject.Models.User
{
    public class UserSummary
    {
        public int Id { get; set; }
        public string DepartmentName { get; set; }
        public string RoleName { get; set; }
        public string FullName { get; set; }
        public string EmailAddress { get; set; }
    }
}
