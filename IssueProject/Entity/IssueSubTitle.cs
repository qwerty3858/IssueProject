namespace IssueProject.Entity
{
    public class IssueSubTitle
    {
        public int Id { get; set; }

        public int TitleId { get; set; }

        public string SubTitle { get; set; }
        public virtual IssueTitle Title { get; set; }
    }
}
