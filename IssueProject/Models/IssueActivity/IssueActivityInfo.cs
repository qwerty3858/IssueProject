using IssueProject.Models.IssueActivityDetail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IssueProject.Models.IssueActivity
{
    public class IssueActivityInfo
    {
        
        public byte Type { get; set; }
        public short SubActivityNo { get; set; }
        public string SubActivityTitle { get; set; }
        public int ParentId { get; set; }
        public virtual ICollection<IssueActivitiyDetailInfo> IssueActivitiyDetailInfos { get; set; }
    }
}
