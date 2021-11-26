#nullable disable

namespace IssueProject.Entity
{
    public partial class IssueAttachment
    {
        public int Id { get; set; }
        public int IssueId { get; set; }
        public string FileName { get; set; }
        public string UniqueName { get; set; }
        public bool Deleted { get; set; }

        public virtual Issue Issue { get; set; }
    }
}
