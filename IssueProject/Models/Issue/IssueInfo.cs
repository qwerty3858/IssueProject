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
       // public string WorkArea { get; set; }
        //public short? IssueNo { get; set; }
        public byte? VersionNo { get; set; }
        public int TitleId { get; set; }
        public int? SubtitleId { get; set; }
        public string Summary { get; set; }
        //public string? Keywords { get; set; }
        public bool IsSaveWithConfirm { get; set; }
        public ActivityStatuses Status { get; set; }
        public virtual List<IssueActivityInfo> IssueActivitiyInfos { get; set; }
        
        public virtual List<IssueAttachmentInfo> IssueAttachmentInfos { get; set; }
        //public virtual List<IssueConfirmInfo>? IssueConfirmInfos { get; set; }
        public virtual List<IssueNoteInfo> IssueNoteInfos { get; set; }
        public virtual List<IssuePreconditionInfo> IssuePreconditionInfos { get; set; }
        public virtual List<IssueRelevantDepartmentInfo> IssueRelevantDepartmentInfos { get; set; }
        public virtual List<IssueRoleInfo> IssueRoleInfos { get; set; }
    }
}
