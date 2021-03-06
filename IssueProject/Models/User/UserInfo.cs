using IssueProject.Models.Department;
using System.Collections.Generic;

namespace IssueProject.Models.User
{
    public class UserInfo
    {
        public int Id { get; set; }
        public int DepartmentId { get; set; }
        public byte RoleId { get; set; }
        public string FullName { get; set; }
        public string DepartmentName { get; set; }
        public string RoleName { get; set; }
        public string Password { get; set; }
        public string EmailAddress { get; set; }
        public bool IsManager { get; set; }
        public bool IsKeyUser { get; set; }
        public List<DepartmentInfo> DepartmentList { get; set; }

    }
}