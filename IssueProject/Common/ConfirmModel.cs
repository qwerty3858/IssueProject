using IssueProject.Models.Issue;

namespace IssueProject.Common
{
    public class ConfirmModel
    {
        public int IssueId { get; set; }
        public string Description { get; set; }
        public IssueInfo issueInfo { get; set; }
    }
}
