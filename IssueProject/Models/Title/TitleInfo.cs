﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IssueProject.Models.Title
{
    public class TitleInfo
    {
        public int Id { get; set; }

        public string Subject { get; set; }

        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }

    }
}
