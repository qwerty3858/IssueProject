#nullable disable

namespace IssueProject.Entity
{
    public partial class IssueActivitiyDetail
    {
        public int Id { get; set; }
        public int IssueActivityId { get; set; }
        public short LineNo { get; set; }
        public string Definition { get; set; }
        public byte RoleId { get; set; }
        public byte Medium { get; set; }
        public string Explanation { get; set; }

        public virtual IssueActivitiy IssueActivity { get; set; }
    }
}
