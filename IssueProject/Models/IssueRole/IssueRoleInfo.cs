#nullable disable

using IssueProject.Models.Role;

namespace IssueProject.Models.IssueRole
{
    public partial class IssueRoleInfo
    {
        //public int Id { get; set; }
        //public int IssueId { get; set; }
        public int RoleId { get; set; }
        public int Id { get; set; }
        public RoleInfo Role { get; set; }
    }
}
