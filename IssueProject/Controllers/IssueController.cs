using IssueProject.Common;
using IssueProject.Models.Issue;
using IssueProject.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.IO;
using IssueProject.Models.SubTitle;

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

        [HttpGet("SuperAdminIssueList")]
        //[Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> IssueList()
        { 
            var vResult = await _issueService.GetList();
            return Ok(vResult);
        }
        [HttpGet("PrivateIssueList")]

        public async Task<IActionResult> IssueListById()
        {
            //string vUserId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            int vUserId = User.GetSubject<int>();
            var vResult = await _issueService.GetListByUserId(vUserId);
            return Ok(vResult);
        }

        [HttpGet("ComeToMeIssues")]

        public async Task<IActionResult> GetListComeToMeIssues()
        {
            // string vUserId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            int vUserId = User.GetSubject<int>();
            var vResult = await _issueService.GetListComeToMeIssues(vUserId);
            return Ok(vResult);
        }

        [HttpGet("IssueInfo/{issueId}")] 
        public async Task<IActionResult> GetSelectedIssue(int issueId)
        { 
            var vResult = await _issueService.SelectedIssueById(issueId);
            return Ok(vResult);
        }

        [HttpGet("VersionInfo/{issueId}")]
        public async Task<IActionResult> GetVersionInfoList(int issueId)
        { 
            var vResult = await _issueService.GetVersionInfoList(issueId);
            return Ok(vResult);
        }
        [HttpGet("VersionSelectedInfo/{issueId}")]
        public async Task<IActionResult> GetVersionInfo(int issueId)
        {
            var vResult = await _issueService.GetVersionSelectedInfo(issueId);
            return Ok(vResult);
        }
        [HttpPost("Add")] 
        public async Task<IActionResult> AddIssue(IssueInfo issueInfo)
        {
            //string vUserId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            int vUserId = User.GetSubject<int>();
            //var file = Request.Form.Files[0];
            var vResult = await _issueService.AddIssue(issueInfo, vUserId);
            return Ok(vResult);
        }
        [HttpPost("AddSubtitle")]
        public async Task<IActionResult> AddSubTitle(SubTitleInfo subtitleInfo)
        {
            var vResult = await _issueService.AddSubtitle(subtitleInfo);
            return Ok(vResult);
        }
        [HttpGet("TitleInfoByDepartmentId/{DepartmentId}")]
        public async Task<IActionResult> TitleInfoByDepartmentId(int DepartmentId)
        {
            var vResult = await _issueService.GetTitleInfoByDepartmentId(DepartmentId);
            return Ok(vResult);
        }
        [HttpGet("TitleInfo/{TitleControl}")]
        public async Task<IActionResult> TitleInfo(bool TitleControl)
        {
            int vUserId = User.GetSubject<int>();
            var vResult = await _issueService.GetTitleInfo(vUserId,TitleControl);
            return Ok(vResult);
        }
        [HttpGet("RejectReason/{issueId}")]
        public async Task<IActionResult> RejectReason(int IssueId)
        {
            var vResult = await _issueService.GetRejectReason(IssueId);
            return Ok(vResult);
        }
        [HttpGet("SubTitleInfo/{TitleId}")]
        public async Task<IActionResult> SubTitleInfo(int TitleId)
        {
            var vResult = await _issueService.GetSubTitleInfo(TitleId);
            return Ok(vResult);
        }

        [HttpPost("Confirm")]
        //[Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Confirmation(ConfirmModel confirmModel)
        {
            int vUserId = User.GetSubject<int>();
            var vResult = await _issueService.Confirm(confirmModel.IssueId, vUserId );
            return Ok(vResult);
        }

        [HttpPost("Upload"), DisableRequestSizeLimit]
        public async Task<IActionResult> FileUpload()
        {
            List<IFormFile> files = (List<IFormFile>)Request.Form.Files;
            var vResult = await _issueService.Upload(files);
            return Ok(vResult);
        }
    
        [HttpPost("Reject")]
        //[Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Rejection(ConfirmModel confirmModel)
        {
            //string vUserId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            int vUserId = User.GetSubject<int>();
            var vResult = await _issueService.Reject(confirmModel.IssueId, vUserId, confirmModel.Description);
            return Ok(vResult);
        }
         
        [HttpDelete("Delete/{Id}")]
        public async Task<IActionResult> DeleteIssue(int Id)
        {
            var vResult = await _issueService.DeleteIssue(Id);
            return Ok(vResult);
        }

    }
}
