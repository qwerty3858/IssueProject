using System;
using System.Collections.Generic;

#nullable disable

namespace IssueProject.Models.IssueAttachment
{
    public partial class IssueAttachmentInfo
    {
        public int Id { get; set; }
        public int IssueId { get; set; }
        public string FileName { get; set; }
        public string UniqueName { get; set; }
        public bool Deleted { get; set; }

    }
}
