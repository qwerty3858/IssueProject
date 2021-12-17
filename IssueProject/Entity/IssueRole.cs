#nullable disable

namespace IssueProject.Entity
{
    public partial class IssueRole
    {
        public int Id { get; set; }
        public int IssueId { get; set; }
        public byte RoleId { get; set; }

        //public virtual Issue Issue { get; set; }
        public virtual Role Role { get; set; }
    }
}
