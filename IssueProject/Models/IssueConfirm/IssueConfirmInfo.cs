using IssueProject.Enums.Confirm;
using System;
using System.Collections.Generic;

#nullable disable

namespace IssueProject.Models.IssueComfirm
{
    public partial class IssueConfirmInfo
    {
        
        public byte VersionNo { get; set; }
        public int DepartmentId { get; set; }
        public int UserId { get; set; }
        public ConfirmStatus Status { get; set; }
        public string Description { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? MailTime { get; set; }
        public DateTime? SubmitTime { get; set; }

    }
}
