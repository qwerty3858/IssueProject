using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace IssueProject.Entity
{
    public class IssueActivitiyDetail
    {
        public int Id { get; set; }
        public int IssueActivitiyId { get; set; }
        public string LineNo { get; set; }
        public string Definition { get; set; }
        public byte RoleId { get; set; }
        public string Medium { get; set; }
        public string Explanation { get; set; }
        public int? ParentId { get; set; }

        public virtual IssueActivitiy IssueActivitiy { get; set; }

        public virtual List<IssueActivitiyDetail> IssueActivitiyDetails { get; set; }
        
      //  public virtual IssueActivitiyDetail Parent { get; set; }
    }
}
