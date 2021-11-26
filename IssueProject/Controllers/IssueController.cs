using IssueProject.Common;
using IssueProject.Models.Issue;
using IssueProject.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IssueProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class IssueController : ControllerBase
    {
        IssueService _issueService;
        IHttpContextAccessor _httpContextAccessor;
        private IHostingEnvironment Environment;
        public IssueController(IssueService issueService, IHttpContextAccessor httpContextAccessor, IHostingEnvironment _environment)
        {
            _issueService = issueService;
            _httpContextAccessor = httpContextAccessor;
            Environment = _environment;
        }
        [HttpPost("Add")]
       
        public async Task<IActionResult> AddIssue(IssueInfo issueInfo)
        {
            string vUserId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var vResult = await _issueService.AddIssue(issueInfo, vUserId);
            return Ok(vResult);
        }

        [HttpGet("PrivateList")]
        //[Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> IssueList()
        {
            
            var vResult = await _issueService.GetList();
            return Ok(vResult);
        }

        [HttpPost("Confirm")]
        
        //[Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Confirmation(ConfirmModel confirmModel)
        {
            string vUserId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var vResult = await _issueService.Confirm(confirmModel.issueRelevantDepartmentId, "1");
            return Ok(vResult);
        }

        [HttpPost("demo")]

        //[Authorize(Roles = "SuperAdmin")]
        public IActionResult demo(IFormFileCollection confirmModel)
        {
            byte[] fileBytes;
            foreach (var attachment in confirmModel)
            {
                using (var ms = new MemoryStream())
                {
                    attachment.CopyTo(ms);
                    fileBytes = ms.ToArray();
                }

            }

            return Ok("EKLENDİ");
        }

        [HttpPost("Reject")]
        //[Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Rejection(ConfirmModel confirmModel)
        {
            string vUserId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var vResult = await _issueService.Reject(confirmModel.issueRelevantDepartmentId, vUserId,confirmModel.description);
            return Ok(vResult);
        }

        [HttpGet("PublicList")]
        
        public async Task<IActionResult> IssueListById()
        {
            string vUserId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var vResult = await _issueService.GetListByUserId(vUserId);
            return Ok(vResult);
        }

        [HttpGet("SelectedList")]
       
        public async Task<IActionResult> GetSelectedList(int issueId)
        {
             
            var vResult = await _issueService.SelectedListById(issueId);
            return Ok(vResult);
        }
    }
}
