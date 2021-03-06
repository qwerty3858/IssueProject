
using IssueProject.Common;
using System;

#nullable disable

namespace IssueProject.Models.IssueComfirm
{
    public partial class IssueConfirmInfo
    {
        
        public int VersionNo { get; set; }
        public int DepartmentId { get; set; }
        public int UserId { get; set; } 
        public ConfirmStatuses Status { get; set; }
        public string Description { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime MailTime { get; set; }
        public DateTime SubmitTime { get; set; }

        //public IssueInfo IssueInfo { get; set; }

    }
}
