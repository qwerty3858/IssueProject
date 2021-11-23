﻿using IssueProject.Models.Department;
using IssueProject.Models.Role;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IssueProject.Models.User
{
    public class UserInfo
    {
        public int Id { get; set; }
        public int DepartmentId { get; set; }
        public byte RoleId { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }
        public string EmailAddress { get; set; }
        public bool Deleted { get; set; }

        public virtual DepartmentInfo DepartmentInfo { get; set; }
        public virtual RoleInfo RoleInfo { get; set; }
    }
}