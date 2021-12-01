using IssueProject.Common;
using IssueProject.Models.Issue;
using IssueProject.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        public IssueController(IssueService issueService, IHttpContextAccessor httpContextAccessor)
        {
            _issueService = issueService;
            _httpContextAccessor = httpContextAccessor;
        }
        [HttpPost("Add")]

        public async Task<IActionResult> AddIssue(IssueInfo issueInfo)
        {
            //string vUserId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            //var file = Request.Form.Files[0];
            var vResult = await _issueService.AddIssue(issueInfo, "1");
            return Ok(vResult);
        }

        [HttpGet("SuperAdminIssueList")]
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
            int vUserId = User.GetSubject<int>();
            var vResult = await _issueService.Confirm(confirmModel.IssueId, vUserId);
            return Ok(vResult);
        }

        [HttpPost("Upload"), DisableRequestSizeLimit]
        public async Task<IActionResult> FileUpload()
        {
            // string vUserId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var file = Request.Form.Files[0];
            var vResult = await _issueService.Upload(file);
            return Ok(vResult);
        }

        [HttpPost("Reject")]
        //[Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Rejection(ConfirmModel confirmModel)
        {
            string vUserId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var vResult = await _issueService.Reject(confirmModel.IssueId, vUserId, confirmModel.Description);
            return Ok(vResult);
        }

        [HttpGet("PrivateIssueList")]

        public async Task<IActionResult> IssueListById()
        {
            string vUserId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var vResult = await _issueService.GetListByUserId(vUserId);
            return Ok(vResult);
        }

        [HttpGet("ComeToMeIssues")]

        public async Task<IActionResult> GetListComeToMeIssues()
        {
            // string vUserId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var vResult = await _issueService.GetListComeToMeIssues("2");
            return Ok(vResult);
        }

        [HttpGet("SelectedIssue/{issueId}")]

        public async Task<IActionResult> GetSelectedIssue(int issueId)
        {

            var vResult = await _issueService.SelectedIssueById(issueId);
            return Ok(vResult);
        }

        [HttpPut("SoftDelete")]
        public async Task<IActionResult> DeleteIssue(int id)
        {
            var vResult = await _issueService.DeleteIssue(id);
            return Ok(vResult);
        }

    }
}
