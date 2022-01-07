using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IssueProject.Models.SubTitle
{
    public class SubtitleSummary
    {
        public int Id { get; set; }
        public int TitleId { get; set; }
        public string Subject { get; set; }
        public string TitleSubject { get; set; }
        public string DepartmentName { get; set; }
        public int DepartmentId { get; set; }
    }
}
