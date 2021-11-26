#nullable disable

namespace IssueProject.Entity
{
    public partial class IssuePrecondition
    {
        public int Id { get; set; }
        public int IssueId { get; set; }
        public short LineNo { get; set; }
        public string Explanation { get; set; }

        public virtual Issue Issue { get; set; }
    }
}
