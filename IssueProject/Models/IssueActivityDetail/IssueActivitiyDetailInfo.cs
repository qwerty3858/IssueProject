#nullable disable

namespace IssueProject.Models.IssueActivityDetail
{
    public partial class IssueActivitiyDetailInfo
    {
        public int ActivityId { get; set; }
        public short LineNo { get; set; }
        public string Definition { get; set; }
        public byte RoleId { get; set; }
        public byte Medium { get; set; }
        public string Explanation { get; set; }
        public int ParentId { get; set; }


    }
}
