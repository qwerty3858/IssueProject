using EmailService;
using IssueProject.Common;
using IssueProject.Entity;
using IssueProject.Entity.Context;
using IssueProject.Enums.Issue;
using IssueProject.Models.Issue;
using IssueProject.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IssueProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IssueController : ControllerBase
    {
        IssueService _issueService;
        IHttpContextAccessor _httpContextAccessor;
        public IssueController(IssueService issueService, IHttpContextAccessor httpContextAccessor)
        {
            _issueService = issueService;
            _httpContextAccessor = httpContextAccessor;
        }
        [HttpPost("Add")]
        public async Task<IActionResult> AddIssue(IssueInfo issueInfo)
        {
            var vResult = await _issueService.AddIssue(issueInfo);
            return Ok(vResult);
        }

        [HttpGet("PrivateList")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> IssueList()
        {
            
            var vResult = await _issueService.GetList();
            return Ok(vResult);
        }


        [HttpGet("PublicList")]
        [Authorize]
        public async Task<IActionResult> IssueListById()
        {
            string vUserId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var vResult = await _issueService.GetListByUserId(vUserId);
            return Ok(vResult);
        }

        [HttpGet("SelectedList")]
        [Authorize]
        public async Task<IActionResult> GetSelectedList(int issueId)
        {
             
            var vResult = await _issueService.SelectedListById(issueId);
            return Ok(vResult);
        }
    }
}
