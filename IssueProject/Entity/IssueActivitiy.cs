using System.Collections.Generic;

#nullable disable

namespace IssueProject.Entity
{
    public partial class IssueActivitiy
    {
        public IssueActivitiy()
        {
            IssueActivitiyDetails = new HashSet<IssueActivitiyDetail>();
        }

        public int Id { get; set; }
        public int IssueId { get; set; }
        public byte Type { get; set; }
        public short SubActivityNo { get; set; }
        public string SubActivityTitle { get; set; }
        public int ParentId { get; set; }

        public virtual Issue Issue { get; set; }
        public virtual ICollection<IssueActivitiyDetail> IssueActivitiyDetails { get; set; }
    }
}
