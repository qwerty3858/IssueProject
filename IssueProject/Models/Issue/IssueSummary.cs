using IssueProject.Models.IssueRelevantDepartMent;
using System.Collections.Generic;

namespace IssueProject.Models.Issue
{
    public class IssueSummary
    {
        public int Id { get; set; }
        public int UserId { get; set; }
       // public string WorkArea { get; set; }
        public string Title { get; set; }
        public string DepartmentName { get; set; }
        public string FullName { get; set; }
        public string RoleName { get; set; }
        public string Summary { get; set; }
        public int Status { get; set; }
        public bool CheckCommit { get; set; }
        public List<IssueRelevantDepartmentInfo> RelevantDepartmentId { get; set; }
    }
}