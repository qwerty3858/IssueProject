
using IssueProject.Common;
using System;
 

namespace IssueProject.Entity
{
    public partial class IssueConfirm
    {
        public int Id { get; set; }
        public int IssueId { get; set; }
        public byte VersionNo { get; set; }
        public int DepartmentId { get; set; }
        public int UserId { get; set; } 
        public ConfirmStatuses Status { get; set; }
        public string? Description { get; set; }
        public DateTime CreateTime { get; set; } = new DateTime();
        public DateTime? MailTime { get; set; }
        public DateTime SubmitTime { get; set; } = DateTime.Now;
        public bool IsConfirm { get; set; }
        public bool IsRejectSend { get; set; }
        public bool IsCommited { get; set; }
        public virtual Issue Issue { get; set; }
        public virtual User User { get; set; }


    }
    


}
