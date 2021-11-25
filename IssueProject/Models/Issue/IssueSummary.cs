using IssueProject.Enums.Confirm;
using IssueProject.Enums.Issue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IssueProject.Models.Issue
{
    public class IssueSummary
    {
        public int Id { get; set; }
        public short WorkArea { get; set; }
        public string DepartmentName { get; set; }
        public string FullName { get; set; }
        public string RoleName { get; set; }
        public int Status { get; set; }
        public ConfirmStatus ConfirmStatus { get; set; }
        
       
    }
}
