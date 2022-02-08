#nullable disable

using System.Collections.Generic;

namespace IssueProject.Models.IssueActivityDetail
{
    public partial class IssueActivitiyDetailInfo
    {
        //public int ActivityId { get; set; }
        public int Id { get; set; }
        public string LineNo { get; set; }
        public string Definition { get; set; }
        public int RoleId { get; set; }
        public string Medium { get; set; }
        public string Explanation { get; set; }
         
        public List<IssueActivitiyDetailInfo> IssueActivityDetailInfos { get; set; }


    }
}
