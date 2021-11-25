using IssueProject.Common;
using IssueProject.Entity.Context;
using IssueProject.Models.Role;
using IssueProject.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IssueProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        RoleService _roleService;

        public RoleController(RoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet("Get")]
        public async Task<IActionResult> GetRoleList()
        {
            //int vUserId = Convert.ToInt32(User.FindFirst("sub").Value);

            var vResult = await _roleService.GetRole();
            return Ok(vResult);
        }
    }
}
