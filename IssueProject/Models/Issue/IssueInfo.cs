using IssueProject.Common;
 
using IssueProject.Models.IssueActivity;
using IssueProject.Models.IssueAttachment;
using IssueProject.Models.IssueNote;
using IssueProject.Models.IssueRelevantDepartMent;
using IssueProject.Models.IssueRole;
using IssueProject.Models.Precondition;
using System.Collections.Generic;

namespace IssueProject.Models.Issue
{
    public class IssueInfo
    {
        public int Id { get; set; }
        public string WorkArea { get; set; }
        //public short? IssueNo { get; set; }
        public byte? VersionNo { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Summary { get; set; }
        //public string? Keywords { get; set; }
        public bool IsSaveWithConfirm { get; set; }
        public ActivityStatuses Status { get; set; }
        public virtual List<IssueActivityInfo> IssueActivitiyInfos { get; set; }
        
        public virtual ICollection<IssueAttachmentInfo> IssueAttachmentInfos { get; set; }
        //public virtual List<IssueConfirmInfo>? IssueConfirmInfos { get; set; }
        public virtual ICollection<IssueNoteInfo> IssueNoteInfos { get; set; }
        public virtual ICollection<IssuePreconditionInfo> IssuePreconditionInfos { get; set; }
        public virtual ICollection<IssueRelevantDepartmentInfo> IssueRelevantDepartmentInfos { get; set; }
        public virtual ICollection<IssueRoleInfo> IssueRoleInfos { get; set; }
    }
}
