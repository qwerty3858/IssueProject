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
using IssueProject.Models.Title;

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

        [HttpGet("PublicIssueList")]
        //[Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> IssueList()
        { 
            var vResult = await _issueService.GetListIssue();
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
        [HttpGet("GetListRelevantIssues")]

        public async Task<IActionResult> GetListRelevantIssues()
        {
            //string vUserId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            int vUserId = User.GetSubject<int>();
            var vResult = await _issueService.GetListRelevantIssues(vUserId);
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
        [HttpPost("Update")]
        public async Task<IActionResult> UpdateIssue(IssueInfo issueInfo)
        {
            //string vUserId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            int vUserId = User.GetSubject<int>();
            //var file = Request.Form.Files[0];
            var vResult = await _issueService.UpdateIssue(issueInfo, vUserId);
            return Ok(vResult);
        }
        [HttpPost("Revision")]
        public async Task<IActionResult> RevisionIssue(IssueInfo issueInfo)
        {
            //string vUserId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            int vUserId = User.GetSubject<int>();
            //var file = Request.Form.Files[0];
            var vResult = await _issueService.RevisionIssue(issueInfo, vUserId);
            return Ok(vResult);
        }
        [HttpPost("AddSubtitle")]
        public async Task<IActionResult> AddSubTitle(SubTitleInfo subtitleInfo)
        {
            var vResult = await _issueService.AddSubtitle(subtitleInfo);
            return Ok(vResult);
        }
        [HttpPost("AddTitle")]
        public async Task<IActionResult> AddTitle(TitleInfo titleInfo)
        {
            var vResult = await _issueService.AddTitle(titleInfo);
            return Ok(vResult);
        }
        [HttpPut("UpdateSubtitle")]
        public async Task<IActionResult> UpdateSubtitle(SubtitleSummary subtitleSummary)
        {
            var vResult = await _issueService.UpdateSubtitle(subtitleSummary);
            return Ok(vResult);
        }
        [HttpPut("UpdateTitle")]
        public async Task<IActionResult> UpdateTitle(SubtitleSummary subtitleSummary)
        {
            var vResult = await _issueService.UpdateTitle(subtitleSummary);
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
        [HttpGet("GetAllSubtitleInfo")]
        public async Task<IActionResult> GetAllSubtitleInfo()
        {
            var vResult = await _issueService.GetAllSubtitleInfo();
            return Ok(vResult);
        }
        [HttpGet("GetAllTitleInfo")]
        public async Task<IActionResult> GetAllTitleInfo()
        {
            var vResult = await _issueService.GetAllTitleInfo();
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
        [HttpDelete("DeleteSubtitle/{Id}")]
        public IActionResult DeleteSubtitle(int Id)
        {

            var vResult = _issueService.DeleteSubtitle(Id);
            return Ok(vResult);
        }
        [HttpDelete("DeleteFile/{fileInfo}/{Id}"),DisableRequestSizeLimit]
        public IActionResult DeleteFile(string fileInfo,int Id)
        {
            
            var vResult = _issueService.DeleteFile(fileInfo, Id);
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
