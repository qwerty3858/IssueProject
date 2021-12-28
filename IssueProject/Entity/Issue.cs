
using IssueProject.Common;
using System.Collections.Generic;


namespace IssueProject.Entity
{
    public partial class Issue
    {
       

        public int Id { get; set; }
        //public string WorkArea { get; set; }
        public int DepartmentId { get; set; }
        public int UserId { get; set; }
        public short IssueNo { get; set; }
        public byte VersionNo { get; set; }
        public int TitleId { get; set; }
        public int? SubtitleId { get; set; }
        public string Summary { get; set; }
        public string Keywords { get; set; }
        public ActivityStatuses Status { get; set; }
        public bool Deleted { get; set; }

        public virtual Department Department { get; set; }
        public virtual User User { get; set; }
        public virtual List<IssueActivitiy> IssueActivitiys { get; set; }
        public virtual List<IssueAttachment> IssueAttachments { get; set; }
        public virtual List<IssueConfirm> IssueConfirms { get; set; }
        public virtual List<IssueNote> IssueNotes { get; set; }
        public virtual List<IssuePrecondition> IssuePreconditions { get; set; }
        public virtual List<IssueRelevantDepartmant> IssueRelevantDepartmants { get; set; }
        public virtual List<IssueRole> IssueRoles { get; set; }
       public virtual IssueTitle Title { get; set; }
       public virtual IssueSubTitle Subtitle { get; set; }

    }

    
}
