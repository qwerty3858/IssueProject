using System.Collections.Generic;

 

namespace IssueProject.Entity
{
    public partial class IssueActivitiy
    {    
        public int Id { get; set; }
        public int IssueId { get; set; }
        public byte Type { get; set; }
        public short SubActivityNo { get; set; }
        public string SubActivityTitle { get; set; }

       // public virtual Issue Issue { get; set; }
        public virtual List<IssueActivitiyDetail> IssueActivitiyDetails { get; set; }
    }
}