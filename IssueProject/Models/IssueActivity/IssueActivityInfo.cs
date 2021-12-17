using IssueProject.Models.IssueActivityDetail;
using System.Collections.Generic;

namespace IssueProject.Models.IssueActivity
{
    public class IssueActivityInfo
    {
       
        public byte Type { get; set; }
        public short SubActivityNo { get; set; }
        public string SubActivityTitle { get; set; }
        //public int ParentId { get; set; }
        
        public virtual List<IssueActivitiyDetailInfo> IssueActivityDetailInfos { get; set; }
    }
}
