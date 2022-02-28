using System.Collections.Generic;

namespace IssueProject.Entity
{
    public class IssueTitle
    {
        public int Id { get; set; }

        public string Subject { get; set; }

        public int DepartmentId { get; set; }

        //public int IssueId { get; set; }
        public virtual Department Department { get; set; }
        //public virtual Issue Issue { get; set; }
        public virtual List<IssueSubTitle> IssueSubTitles { get; set; } 
    }
}
