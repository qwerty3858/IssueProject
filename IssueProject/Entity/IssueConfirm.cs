using System;
using System.Collections.Generic;

#nullable disable

namespace IssueProject.Entity
{
    public partial class IssueConfirm
    {
        public int Id { get; set; }
        public int IssueId { get; set; }
        public byte VersionNo { get; set; }
        public int DepartmentId { get; set; }
        public int UserId { get; set; }
        public byte Status { get; set; }
        public string Description { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? MailTime { get; set; }
        public DateTime? SubmitTime { get; set; }

        public virtual Issue Issue { get; set; }
    }
}
