using IssueProject.Enums.Issue;
using System.Collections.Generic;

#nullable disable

namespace IssueProject.Entity
{
    public partial class Issue
    {
        public Issue()
        {
            IssueActivitiys = new HashSet<IssueActivitiy>();
           // IssueAttachments = new HashSet<IssueAttachment>();
            //IssueConfirms = new HashSet<IssueConfirm>();
            IssueNotes = new HashSet<IssueNote>();
            IssuePreconditions = new HashSet<IssuePrecondition>();
            IssueRelevantDepartmants = new HashSet<IssueRelevantDepartmant>();
            IssueRoles = new HashSet<IssueRole>();
        }

        public int Id { get; set; }
        public short WorkArea { get; set; }
        public int DepartmentId { get; set; }
        public int UserId { get; set; }
        public short? IssueNo { get; set; }
        public byte? VersionNo { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Summary { get; set; }
        public string? Keywords { get; set; }
        public Status Status { get; set; }
        public bool Deleted { get; set; }

        public virtual Department Department { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<IssueActivitiy>? IssueActivitiys { get; set; }
        public virtual List<IssueAttachment>? IssueAttachments { get; set; }
        public virtual List<IssueConfirm> IssueConfirms { get; set; }
        public virtual ICollection<IssueNote> IssueNotes { get; set; }
        public virtual ICollection<IssuePrecondition> IssuePreconditions { get; set; }
        public virtual ICollection<IssueRelevantDepartmant> IssueRelevantDepartmants { get; set; }
        public virtual ICollection<IssueRole> IssueRoles { get; set; }
    }

    
}
