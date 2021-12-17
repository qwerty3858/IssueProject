 

namespace IssueProject.Entity
{
    public partial class IssueRelevantDepartmant
    {
        public int Id { get; set; }
        public int IssueId { get; set; }
        public int DepartmentId { get; set; }

        public virtual Department Department { get; set; }
        //public virtual Issue Issue { get; set; }
    }
}
