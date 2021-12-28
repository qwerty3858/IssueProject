using EmailService;
using IssueProject.Common;
using IssueProject.Entity;
using IssueProject.Entity.Context;

using IssueProject.Models.Issue;
using IssueProject.Models.IssueActivityDetail;
using IssueProject.Models.IssueComfirm;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AutoMapper;
using IssueProject.Models.Title;
using IssueProject.Models.SubTitle;
using IssueProject.Models.Version;
using IssueProject.Models.RejectReason;
using IssueProject.Models.IssueAttachment;

namespace IssueProject.Services
{
    public class IssueService
    {
        private ILogger<IssueService> _logger;

        private IEmailSender _emailSender;

        private _2Mes_ConceptualContext _context;
        IMapper _mapper;
        public IssueService(_2Mes_ConceptualContext context, IEmailSender emailSender, ILogger<IssueService> logger, IMapper mapper)
        {
            _context = context;
            _emailSender = emailSender;
            _logger = logger;
            _mapper = mapper;

        }
        public async Task<Result<IssueSubTitle>> AddSubtitle(SubTitleInfo subTitleInfo)
        {
            try
            {
                var vSubtitleInfo = new IssueSubTitle
                {
                    TitleId = subTitleInfo.Id,
                    SubTitle = subTitleInfo.SubTitle
                };
                await _context.IssueSubTitles.AddAsync(vSubtitleInfo);
                await _context.SaveChangesAsync();
                return Result<IssueSubTitle>.PrepareSuccess(vSubtitleInfo);
            }
            catch (Exception vEx)
            {
                _logger.LogError(vEx, "Title Add Error");
                return Result<IssueSubTitle>.PrepareFailure(vEx.Message);
            }
        }
        public async Task<Result<List<TitleInfo>>> GetTitleInfoByDepartmentId(int DepartmentId)
        {
            try
            {
                var vTitleInfos = await _context.IssueTitles.Where(x => x.DepartmentId == DepartmentId)
                    .Select(x => new TitleInfo
                    {
                        Id = x.Id,
                        Subject = x.Subject
                    }).ToListAsync();

                if (vTitleInfos == null)
                {
                    _logger.LogInformation("İstenilen Title için veri bulunamadı. ");
                    return Result<List<TitleInfo>>.PrepareFailure("İstenilen Title için veri bulunamadı.");
                }
                return Result<List<TitleInfo>>.PrepareSuccess(vTitleInfos);
            }
            catch (Exception vEx)
            {
                _logger.LogError(vEx, "Title Info By DepartmentId List Error");
                return Result<List<TitleInfo>>.PrepareFailure(vEx.Message);
            }
        }
        public async Task<Result<List<TitleInfo>>> GetTitleInfo(int UserId, bool TitleControl)
        {
            try
            {
                if (TitleControl)
                {
                    var vUser = _context.Users.FirstOrDefault(x => x.Id == UserId);
                    var vResult = await _context.IssueTitles.Where(x => x.DepartmentId == vUser.DepartmentId || x.DepartmentId == 0)
                        .Select(x => new TitleInfo
                        {
                            Id = x.Id,
                            Subject = x.Subject
                        }).ToListAsync();

                    if (vResult == null)
                    {
                        _logger.LogInformation("İstenilen Title için veri bulunamadı. ");
                        return Result<List<TitleInfo>>.PrepareFailure("İstenilen Title için veri bulunamadı.");
                    }
                    return Result<List<TitleInfo>>.PrepareSuccess(vResult);
                }
                else
                {
                    var vResult = await _context.IssueTitles
                        .Select(x => new TitleInfo
                        {
                            Id = x.Id,
                            Subject = x.Subject
                        }).ToListAsync();

                    if (vResult == null)
                    {
                        _logger.LogInformation("İstenilen Title için veri bulunamadı. ");
                        return Result<List<TitleInfo>>.PrepareFailure("İstenilen Title için veri bulunamadı.");
                    }
                    return Result<List<TitleInfo>>.PrepareSuccess(vResult);
                }

            }
            catch (Exception vEx)
            {
                _logger.LogError(vEx, "Title Info List Error");
                return Result<List<TitleInfo>>.PrepareFailure(vEx.Message);
            }
        }
        public async Task<Result<List<SubTitleInfo>>> GetSubTitleInfo(int TitleId)
        {
            try
            {
                var vResult = await _context.IssueSubTitles.Where(x => x.TitleId == TitleId).Select(x => new SubTitleInfo
                {
                    Id = x.Id,
                    SubTitle = x.SubTitle
                }).ToListAsync();
                return Result<List<SubTitleInfo>>.PrepareSuccess(vResult);
            }
            catch (Exception vEx)
            {
                _logger.LogError(vEx, "SubTitle Info List Error");
                return Result<List<SubTitleInfo>>.PrepareFailure(vEx.Message);
            }
        }
        public async Task<Result<RejectReasonInfo>> GetRejectReason(int IssueId)
        {
            try
            {

                var vResult = await _context.IssueConfirms
                    .Include(x => x.User)
                    .Where(x => x.IssueId == IssueId && x.Status == ConfirmStatuses.Rejected)
                    .Select(x => new RejectReasonInfo
                    {
                        Description = x.Description,
                        SubmitTime = x.SubmitTime.ToString("dd MMMM yyyy, HH:mm"),
                        FullName = x.User.FullName
                    })
                  .FirstOrDefaultAsync();

                if (vResult == null)
                {
                    _logger.LogInformation("İstenilen Red Sebebi bulunamadı. ");
                    return Result<RejectReasonInfo>.PrepareFailure("İstenilen Red Sebebi bulunamadı.");
                }
                return Result<RejectReasonInfo>.PrepareSuccess(vResult);
            }
            catch (Exception vEx)
            {
                _logger.LogError(vEx, "Reject Reason Info Error");
                return Result<RejectReasonInfo>.PrepareFailure(vEx.Message);
            }
        }

        public async Task<Result<List<VersionInfo>>> GetVersionInfoList(int IssueId)
        {
            try
            {
                var vIssue = await _context.Issues.FirstOrDefaultAsync(x => x.Id == IssueId);
                if (vIssue.IssueNo != 0)
                {
                    var vVersionInfos = await _context.Issues.Where(x => x.IssueNo == vIssue.IssueNo || x.Id == vIssue.IssueNo).ToListAsync();

                    var VersionInfo = vVersionInfos.Select(x => new VersionInfo
                    {
                        Id = x.Id,
                        VersionNo = x.VersionNo,
                        IssueNo = x.IssueNo
                    });


                    //List<IssueInfo> issueInfo = _mapper.Map<List<IssueInfo>>(vVersonInfos);
                    return Result<List<VersionInfo>>.PrepareSuccess(VersionInfo.ToList());
                }
                return Result<List<VersionInfo>>.PrepareFailure("Version Bilgisi Bulunamadı");

            }
            catch (Exception vEx)
            {
                _logger.LogError(vEx, "Version Info List Error");
                return Result<List<VersionInfo>>.PrepareFailure(vEx.Message);
            }
        }

        public async Task<Result<List<IssueInfo>>> GetVersionSelectedInfo(int IssueId)
        {
            try
            {
                var vVersonInfos = await _context.Issues
                    .Include(x => x.IssueActivitiys)
                      .ThenInclude(x => x.IssueActivitiyDetails)
                      .Include(x => x.IssuePreconditions)
                      .Include(x => x.IssueNotes)
                      .Include(x => x.IssueRelevantDepartmants)
                      .ThenInclude(x => x.Department)
                      .Include(x => x.IssueAttachments)
                      .Include(x => x.IssueRoles)
                      .ThenInclude(x => x.Role)
                      .FirstOrDefaultAsync(x => x.Id == IssueId);
                List<IssueInfo> issueInfo = _mapper.Map<List<IssueInfo>>(vVersonInfos);
                return Result<List<IssueInfo>>.PrepareSuccess(issueInfo);
            }
            catch (Exception vEx)
            {
                _logger.LogError(vEx, "Version Info Select Error");
                return Result<List<IssueInfo>>.PrepareFailure(vEx.Message);
            }
        }
        public async Task<Result<IssueInfo>> SelectedIssueById(int IssueId)
        {
            try
            {
                var vIssueActivitys = _context.IssueActivitiys.Include(x => x.IssueActivitiyDetails).Where(x => x.IssueId == IssueId).ToList();


                List<IssueActivitiy> IssueActivitiys = new List<IssueActivitiy>();
                foreach (var item in vIssueActivitys)
                {
                    var vIssueActivity = new IssueActivitiy
                    {
                        Type = item.Type,
                        SubActivityNo = item.SubActivityNo,
                        SubActivityTitle = item.SubActivityTitle,
                        IssueActivitiyDetails = new List<IssueActivitiyDetail>()
                    };


                    foreach (var vSubItem in item.IssueActivitiyDetails.OrderBy(x => x.ParentId))
                    {
                        if (vSubItem.ParentId == null || vSubItem.ParentId == 0)
                            vIssueActivity.IssueActivitiyDetails.Add(vSubItem);
                        else
                        {
                            var vParentItem = vIssueActivity.IssueActivitiyDetails.FirstOrDefault(x => x.Id == vSubItem.ParentId);
                            if (vParentItem != null)
                            {
                                if (vParentItem.IssueActivitiyDetails.All(x => x.Id != vSubItem.Id))
                                    vParentItem.IssueActivitiyDetails.Add(vSubItem);
                            }

                        }
                    }

                    // vIssueActivity.IssueActivitiyDetails = GetIssueActivitiyDetails(vIssueActivity, null, item.IssueActivitiyDetails);

                    IssueActivitiys.Add(vIssueActivity);
                }

                var vIssue = await _context.Issues
                            .Include(x => x.IssuePreconditions)
                            .Include(x => x.IssueNotes)
                            .Include(x => x.IssueRelevantDepartmants)
                            .ThenInclude(x => x.Department)
                            .Include(x => x.IssueAttachments)
                            .Include(x => x.IssueRoles)
                            .ThenInclude(x => x.Role)
                            .FirstOrDefaultAsync(x => x.Id == IssueId);

                vIssue.IssueActivitiys = IssueActivitiys.ToList();
                IssueInfo issueInfo = _mapper.Map<IssueInfo>(vIssue);

                if (issueInfo == null)
                    return Result<IssueInfo>.PrepareFailure("İstenilen sorguya ait veri bulunamadı.");

                return Result<IssueInfo>.PrepareSuccess(issueInfo);
            }
            catch (Exception vEx)
            {
                _logger.LogError(vEx, "Selected Issue Info Error");
                return Result<IssueInfo>.PrepareFailure(vEx.Message);
            }
        }

        public async Task<Result<List<IssueAttachmentInfo>>> Upload(List<IFormFile> files)
        {
            try
            {
                // List<IssueAttachment> vIssueAttachment;
                var folderName = Path.Combine(@"d:\Resources", "temp");

                var vIssueAttachmentInfos = new List<IssueAttachmentInfo>();
                foreach (IFormFile file in files)
                {
                    if (file.Length < 0)
                        return Result<List<IssueAttachmentInfo>>.PrepareFailure("Dosya boyutu hatası");

                    var vAttachment = new IssueAttachmentInfo
                    {
                        FileName = file.FileName,
                        UniqueName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName)
                    };
                    
                    var fullPath = Path.Combine(folderName, vAttachment.UniqueName);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    vIssueAttachmentInfos.Add(vAttachment);
                }

                return Result<List<IssueAttachmentInfo>>.PrepareSuccess(vIssueAttachmentInfos);
            }
            catch (Exception vEx)
            {
                _logger.LogError(vEx, "File Upload Error");
                return Result<List<IssueAttachmentInfo>>.PrepareFailure("Dosya yükleme hatası");
            }
        }

        public async Task<Result<List<IssueSummary>>> GetList()
        {
            try
            {
                var vIssues = await _context.Issues
                    .Include(x => x.Department)
                    .Include(x => x.User)
                    .ThenInclude(x => x.Role)
                    .Where(x => x.Deleted == false)
                            .Select(x => new IssueSummary
                            {
                                Id = x.Id,
                                //WorkArea = x.WorkArea,
                                DepartmentName = x.Department.Definition,
                                FullName = x.User.FullName,
                                RoleName = x.User.Role.Definition,
                                Status = ((int)x.Status),
                                //RelevantDepartmentId = new List<Models.IssueRelevantDepartMent.IssueRelevantDepartmentInfo>
                                //{
                                //     new Models.IssueRelevantDepartMent.IssueRelevantDepartmentInfo
                                //     {
                                //        DepartmentId = x.DepartmentId
                                //     }
                                //}

                            }).ToListAsync();
                if (vIssues == null)
                {
                    _logger.LogInformation("İstenilen sorguya ait veri bulunamadı. ");
                    return Result<List<IssueSummary>>.PrepareFailure("İstenilen sorguya ait veri bulunamadı.");
                }
                return Result<List<IssueSummary>>.PrepareSuccess(vIssues);
            }
            catch (Exception vEx)
            {
                _logger.LogError(vEx, "Issue List Error");
                return Result<List<IssueSummary>>.PrepareFailure(vEx.Message);
            }
        }

        public async Task<Result<List<IssueSummary>>> GetListByUserId(int userId)
        {
            try
            {
                var vIssues = await _context.Issues
                    .Include(x => x.IssueRelevantDepartmants)
                    .Include(x => x.Title)
                    .Include(x => x.Subtitle)
                    .Include(x => x.Department)
                    .Include(x => x.User)
                    .ThenInclude(x => x.Role)
                    .Where(x => x.UserId == userId && x.Deleted == false).OrderByDescending(x => x.Id)
                    .Select(x => new IssueSummary
                    {
                        Id = x.Id,
                        // WorkArea = x.WorkArea,
                        DepartmentName = x.Department.Definition,
                        FullName = x.User.FullName,
                        RoleName = x.User.Role.Definition,
                        Status = ((int)x.Status),
                        Title = x.Title.Subject
                    }).ToListAsync();
                if (vIssues == null)
                {
                    _logger.LogInformation("İstenilen sorguya ait veri bulunamadı. ");
                    return Result<List<IssueSummary>>.PrepareFailure("İstenilen sorguya ait veri bulunamadı.");
                }
                return Result<List<IssueSummary>>.PrepareSuccess(vIssues);
            }
            catch (Exception vEx)
            {
                _logger.LogError(vEx, "Private Issue List Error");
                return Result<List<IssueSummary>>.PrepareFailure(vEx.Message);
            }
        }
        public async Task<Result<List<IssueSummary>>> GetListComeToMeIssues(int userId)
        {
            try
            {
                var vUser = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId && x.Deleted == false);
                if (vUser == null)
                {
                    _logger.LogInformation("İstenilen sorguya ait veri bulunamadı. ");
                    return Result<List<IssueSummary>>.PrepareFailure("İstenilen sorguya ait veri bulunamadı.");
                }

                var vIssues = await _context.IssueConfirms
                    .Include(x => x.Issue)
                    .ThenInclude(x => x.User)
                    .ThenInclude(x => x.Role)
                    .Include(x => x.Issue)
                    .ThenInclude(x => x.Title)
                    .Where(x => x.Issue.Status != ActivityStatuses.Rejected && x.UserId == vUser.Id
                                && (x.Status == ConfirmStatuses.MailSent || x.Status == ConfirmStatuses.MailSendWaiting))
                    .Select(x => new IssueSummary
                    {
                        Id = x.Issue.Id,
                        // WorkArea = x.Issue.WorkArea,
                        DepartmentName = x.Issue.Department.Definition,
                        FullName = x.Issue.User.FullName,
                        RoleName = x.Issue.User.Role.Definition,
                        Status = (int)x.Issue.Status,
                        Title = x.Issue.Title.Subject

                    }).ToListAsync();

                if (vIssues.Count == 0)
                {
                    return Result<List<IssueSummary>>.PrepareFailure("Size Gelen Issue Bilgisi Bulunamadı.");
                }
                return Result<List<IssueSummary>>.PrepareSuccess(vIssues);
            }
            catch (Exception vEx)
            {
                _logger.LogError(vEx, "Come To Me Issue List Error");
                return Result<List<IssueSummary>>.PrepareFailure(vEx.Message);
            }
        }

        public async Task<Result> Reject(int IssueId, int vUserId, string Description)
        {
            try
            {
                var Issue = await _context.Issues
                  .Include(x => x.IssueConfirms)
                  .Include(x => x.User)
                  .FirstOrDefaultAsync(x => x.Id == IssueId);

                if (Issue == null)
                {
                    _logger.LogInformation("İstenilen Issue sorgusuna ait veri bulunamadı. ");
                    return Result<Issue>.PrepareFailure("İstenilen Issue sorgusuna ait veri bulunamadı.");
                }

                if (Issue.Status == ActivityStatuses.Rejected || Issue.Status != ActivityStatuses.Processing)
                {
                    if (Issue.IssueConfirms.All(y => y.UserId != vUserId))
                        return Result<Issue>.PrepareFailure("Reddetmek istediğiniz talep için izniniz bulunmamaktadır!");
                }

                foreach (var vConfirm in Issue.IssueConfirms)
                {

                    if (vConfirm.UserId == vUserId)
                    {
                        vConfirm.IsConfirm = true;
                        vConfirm.SubmitTime = DateTime.Now;
                        vConfirm.Status = ConfirmStatuses.Rejected;
                        vConfirm.Description = Description;
                        vConfirm.IsRejectSend = true;
                    }
                }

                Issue.Status = ActivityStatuses.Rejected;

                await _context.SaveChangesAsync();

                return Result<Issue>.PrepareSuccess(Issue);
            }
            catch (Exception vEx)
            {
                _logger.LogError(vEx, "Reject Error");
                return Result<List<IssueSummary>>.PrepareFailure(vEx.Message);
            }
        }

        public async Task<Result<Issue>> Confirm(int IssueId, int UserId)
        {
            try
            {
                var Issue = await _context.Issues
                    .Include(x => x.User)
                    .Include(x => x.IssueConfirms)
                    .Include(x => x.IssueRelevantDepartmants)
                    .ThenInclude(x => x.Department)
                    .ThenInclude(x => x.Users)
                    .Include(x => x.Department)
                    .ThenInclude(x => x.Users)
                    .FirstOrDefaultAsync(x => x.Id == IssueId);

                if (Issue == null)
                    return Result<Issue>.PrepareFailure("İstenilen sorguya ait veri bulunamadı.");

                if (Issue.Status == ActivityStatuses.Rejected || Issue.Status != ActivityStatuses.Processing)
                {
                    if (Issue.IssueConfirms.All(x => x.UserId != UserId))
                        return Result<Issue>.PrepareFailure("Onaylamak istediğiniz talep için izniniz bulunmamaktadır!");
                }

                switch (Issue.Status)
                {
                    case ActivityStatuses.Processing:
                        {
                            var vBimDepartment = await _context.Departments
                               .Include(x => x.Users)
                               .FirstOrDefaultAsync(x => x.Definition == "Bilgi İşlem");

                            foreach (var vUser in vBimDepartment.Users)
                            {
                                if (vUser.IsKeyUser && !vUser.Deleted)
                                {
                                    _context.IssueConfirms.Add(new IssueConfirm
                                    {
                                        IssueId = IssueId,
                                        VersionNo = Issue.VersionNo,
                                        DepartmentId = vUser.DepartmentId,
                                        UserId = vUser.Id,
                                        Status = ConfirmStatuses.MailSendWaiting,
                                        CreateTime = DateTime.Now,
                                        SubmitTime = DateTime.Now,
                                        IsConfirm = false,
                                        IsRejectSend = false,
                                        IsCommited = false
                                    });
                                }

                            }


                            Issue.Status = ActivityStatuses.ITWaiting;
                            break;
                        }
                    case ActivityStatuses.ITWaiting:
                        {
                            foreach (var vConfirm in Issue.IssueConfirms.ToList())
                            {
                                if (vConfirm.UserId == UserId)
                                {
                                    vConfirm.Status = ConfirmStatuses.Commited;
                                    vConfirm.IsConfirm = true;
                                    vConfirm.SubmitTime = DateTime.Now;

                                    var vHasWaitingCommit = Issue.IssueConfirms
                                        .Any(x => x.UserId != UserId &&
                                        (x.Status == ConfirmStatuses.MailSendWaiting || x.Status == ConfirmStatuses.MailSent));

                                    if (!vHasWaitingCommit)
                                    {
                                        foreach (var vDepartment in Issue.IssueRelevantDepartmants)
                                            foreach (var vUser in vDepartment.Department.Users)
                                            {
                                                if (vUser.IsKeyUser && !vUser.Deleted)
                                                {
                                                    _context.IssueConfirms.Add(new IssueConfirm
                                                    {
                                                        IssueId = IssueId,
                                                        VersionNo = Issue.VersionNo,
                                                        DepartmentId = vDepartment.DepartmentId,
                                                        UserId = vUser.Id,
                                                        Status = ConfirmStatuses.MailSendWaiting,
                                                        CreateTime = DateTime.Now,
                                                        IsConfirm = false,
                                                        IsRejectSend = false,
                                                        IsCommited = false
                                                    });
                                                }
                                            }

                                        foreach (var vUser in Issue.Department.Users)
                                        {
                                            if (vUser.IsKeyUser && !vUser.IsManager && vUser.Id != Issue.UserId && !vUser.Deleted)
                                            {
                                                _context.IssueConfirms.Add(new IssueConfirm
                                                {
                                                    IssueId = IssueId,
                                                    VersionNo = Issue.VersionNo,
                                                    DepartmentId = vUser.DepartmentId,
                                                    UserId = vUser.Id,
                                                    Status = ConfirmStatuses.MailSendWaiting,
                                                    CreateTime = DateTime.Now,
                                                    IsConfirm = false,
                                                    IsRejectSend = false,
                                                    IsCommited = false
                                                });
                                            }
                                        }
                                        Issue.Status = ActivityStatuses.DepartmentWaiting;
                                    }
                                }

                            }
                            break;
                        }
                    case ActivityStatuses.DepartmentWaiting:
                        {
                            foreach (var vConfirm in Issue.IssueConfirms.ToList())
                            {
                                if (vConfirm.UserId == UserId)
                                {
                                    vConfirm.Status = ConfirmStatuses.Commited;
                                    vConfirm.IsConfirm = true;
                                    vConfirm.SubmitTime = DateTime.Now;

                                    var vHasWaitingCommit = Issue.IssueConfirms
                                        .Any(x => x.UserId != UserId &&
                                        (x.Status == ConfirmStatuses.MailSendWaiting || x.Status == ConfirmStatuses.MailSent));

                                    if (!vHasWaitingCommit)
                                    {
                                        foreach (var vUser in Issue.Department.Users)
                                        {
                                            if (vUser.IsManager && !vUser.Deleted)
                                            {
                                                _context.IssueConfirms.Add(new IssueConfirm
                                                {
                                                    IssueId = IssueId,
                                                    VersionNo = Issue.VersionNo,
                                                    DepartmentId = Issue.DepartmentId,
                                                    UserId = vUser.Id,
                                                    Status = ConfirmStatuses.MailSendWaiting,
                                                    CreateTime = DateTime.Now,
                                                    IsConfirm = false,
                                                    IsRejectSend = false,
                                                    IsCommited = false
                                                });
                                            }
                                        }

                                        Issue.Status = ActivityStatuses.ManagerWaiting;
                                    }
                                }

                            }
                            break;
                        }
                    case ActivityStatuses.ManagerWaiting:
                        {
                            foreach (var vConfirm in Issue.IssueConfirms)
                            {
                                if (vConfirm.UserId == UserId)
                                {
                                    vConfirm.Status = ConfirmStatuses.Commited;
                                    vConfirm.IsConfirm = true;
                                    vConfirm.SubmitTime = DateTime.Now;

                                    var vHasWaitingCommit = Issue.IssueConfirms
                                          .Any(x => x.UserId != UserId &&
                                          (x.Status == ConfirmStatuses.MailSendWaiting || x.Status == ConfirmStatuses.MailSent));

                                    if (!vHasWaitingCommit)
                                    {
                                        vConfirm.IsCommited = true;
                                        Issue.Status = ActivityStatuses.ManagerCommitted;
                                    }

                                }
                            }
                            break;
                        }

                    default:
                        {
                            Issue.Status = ActivityStatuses.Locked;
                            break;
                        }
                }

                await _context.SaveChangesAsync();

                return Result<Issue>.PrepareSuccess(Issue);
            }
            catch (Exception vEx)
            {
                _logger.LogError(vEx, "Confirm Error");
                return Result<Issue>.PrepareFailure(vEx.Message);
            }

        }

        public async Task<Result<Issue>> AddIssue(IssueInfo issueInfo, int UserId)
        {
            try
            {
                var vUser = _context.Users.FirstOrDefault(x => x.Id == UserId && x.Deleted == false);
                if (vUser == null)
                {
                    return Result<Issue>.PrepareFailure("User Bulunamadı.");
                }

                var vRevelantDepartment = issueInfo.IssueRelevantDepartmentInfos.Select(x => new IssueRelevantDepartmant
                {
                    DepartmentId = x.DepartmentId,


                });

                var vPrecondition = issueInfo.IssuePreconditionInfos.Select(x => new IssuePrecondition
                {
                    LineNo = x.LineNo,
                    Explanation = x.Explanation,


                });

                var vIssueNote = issueInfo.IssueNoteInfos.Select(x => new IssueNote
                {
                    LineNo = x.LineNo,
                    Explanation = x.Explanation,

                });
                List<IssueActivitiy> vIssueActivitiys = new List<IssueActivitiy>();


                foreach (var vIssueActivityInfo in issueInfo.IssueActivitiyInfos)
                {
                    var vIssueActivity = new IssueActivitiy
                    {
                        Type = vIssueActivityInfo.Type,
                        SubActivityNo = vIssueActivityInfo.SubActivityNo,
                        SubActivityTitle = vIssueActivityInfo.SubActivityTitle,

                    };

                    vIssueActivity.IssueActivitiyDetails = GetIssueActivitiyDetails(vIssueActivity, null, vIssueActivityInfo.IssueActivityDetailInfos);

                    vIssueActivitiys.Add(vIssueActivity);
                }



                var vIssueRole = issueInfo.IssueRoleInfos.Select(x => new IssueRole
                {
                    RoleId = (byte)(x.RoleId),

                });

                var vAttachment = issueInfo.IssueAttachmentInfos.Select(x => new IssueAttachment
                {
                    Deleted = false,
                    FileName = x.FileName,
                    UniqueName = x.UniqueName,
                });

                if (issueInfo.Id > 0)
                {

                    if (issueInfo.Status == ActivityStatuses.Processing)
                    {
                        var vIssue = _context.Issues
                          .Include(x => x.IssueActivitiys)
                          .ThenInclude(x => x.IssueActivitiyDetails)
                          .Include(x => x.IssuePreconditions)
                          .Include(x => x.IssueNotes)
                          .Include(x => x.IssueRelevantDepartmants)
                          // .ThenInclude(x => x.Department)
                          .Include(x => x.IssueAttachments)
                          .Include(x => x.IssueRoles)
                      //.ThenInclude(x => x.Role)
                      .FirstOrDefault(x => x.Id == issueInfo.Id);

                        if (vIssue == null)
                            return Result<Issue>.PrepareFailure("İstenilen sorguya ait veri bulunamadı.");

                        // var vIssueTitle = _context.IssueTitles.FirstOrDefault(x =>x.Id == Int32.Parse(issueInfo.Title));
                        vIssue.TitleId = issueInfo.TitleId;
                        // vIssue.WorkArea = issueInfo.WorkArea; 
                        vIssue.SubtitleId = issueInfo.SubtitleId;
                        vIssue.Summary = issueInfo.Summary;
                        vIssue.Status = ActivityStatuses.Processing;
                        vIssue.IssueActivitiys = vIssueActivitiys.ToList();
                        vIssue.IssueNotes = vIssueNote.ToList();
                        vIssue.IssuePreconditions = vPrecondition.ToList();
                        vIssue.IssueRelevantDepartmants = vRevelantDepartment.ToList();
                        vIssue.IssueRoles = vIssueRole.ToList();
                        if (vAttachment.Any())
                        {
                            string tempPath = @"d:\Resources\temp";
                            string filePath = @"d:\Resources\Files";

                            if (!Directory.Exists(filePath))
                                Directory.CreateDirectory(filePath);

                            foreach (var file in vAttachment)
                            {
                                var vAttachmentFile = _context.IssueAttachments.FirstOrDefault(x => x.UniqueName == file.UniqueName);
                                if (vAttachmentFile == null)
                                {
                                    var tempFilePath = Path.Combine(tempPath, file.UniqueName);
                                    var newFilePath = Path.Combine(filePath, file.UniqueName);

                                    //if (!File.Exists(tempFilePath))
                                    //    continue;

                                    //if (File.Exists(newFilePath))
                                    //    continue;

                                    File.Move(tempFilePath, newFilePath);
                                    vIssue.IssueAttachments.Add(file);
                                }

                            }
                        }
                        
                        vIssue.IssueNo = 0;
                        vIssue.VersionNo = 0;
                        await _context.SaveChangesAsync();
                        if (issueInfo.IsSaveWithConfirm)
                            await Confirm(issueInfo.Id, UserId);
                        return Result<Issue>.PrepareSuccess(vIssue);
                    }

                    else
                    {
                        var vIssue = _context.Issues.AsNoTracking()
                          .Include(x => x.IssueActivitiys)
                          .ThenInclude(x => x.IssueActivitiyDetails)
                          .Include(x => x.IssuePreconditions)
                          .Include(x => x.IssueNotes)
                          .Include(x => x.IssueRelevantDepartmants)
                          // .ThenInclude(x => x.Department)
                          .Include(x => x.IssueAttachments)
                          .Include(x => x.IssueRoles)
                      //.ThenInclude(x => x.Role)
                      .FirstOrDefault(x => x.Id == issueInfo.Id);

                        if (vIssue == null)
                            return Result<Issue>.PrepareFailure("İstenilen sorguya ait veri bulunamadı.");

                        // var vIssueTitle = _context.IssueTitles.FirstOrDefault(x =>x.Id == Int32.Parse(issueInfo.Title));
                        vIssue.TitleId = issueInfo.TitleId;
                        // vIssue.WorkArea = issueInfo.WorkArea; 
                        vIssue.SubtitleId = issueInfo.SubtitleId;
                        vIssue.Summary = issueInfo.Summary;
                        vIssue.Status = ActivityStatuses.Processing;
                        vIssue.IssueActivitiys = vIssueActivitiys.ToList();
                        vIssue.IssueNotes = vIssueNote.ToList();
                        vIssue.IssuePreconditions = vPrecondition.ToList();
                        vIssue.IssueRelevantDepartmants = vRevelantDepartment.ToList();
                        vIssue.IssueRoles = vIssueRole.ToList();
                        vIssue.IssueAttachments = vAttachment.ToList();

                        if (vIssue.IssueNo != 0)
                        {
                            var vIssueNoInfos = await _context.Issues.FirstOrDefaultAsync(x => x.Id == vIssue.IssueNo);
                            if (vIssueNoInfos == null)
                                return Result<Issue>.PrepareFailure("Reddedilme işlemi sırasında IssueNo bilgisi bulunamadı.");
                            vIssue.IssueNo = (short)vIssueNoInfos.Id;
                            vIssue.VersionNo += 1;
                        }
                        else
                        {
                            issueInfo.VersionNo += 1;
                            vIssue.IssueNo = (short)issueInfo.Id;
                            vIssue.VersionNo = (byte)issueInfo.VersionNo;
                        }
                        vIssue.Id = 0;

                        await _context.Issues.AddAsync(vIssue);
                        await _context.SaveChangesAsync();

                        if (vAttachment.Any())
                        {
                            string tempPath = @"d:\Resources\temp";
                            string filePath = @"d:\Resources\Files";

                            if (!Directory.Exists(filePath))
                                Directory.CreateDirectory(filePath);

                            foreach (var file in vAttachment)
                            {
                                var tempFilePath = Path.Combine(tempPath, file.UniqueName);
                                var newFilePath = Path.Combine(filePath, file.UniqueName);

                                //if (!File.Exists(tempFilePath))
                                //    continue;

                                //if (File.Exists(newFilePath))
                                //    continue;

                                File.Move(tempFilePath, newFilePath);
                            }
                        }

                        if (issueInfo.IsSaveWithConfirm)
                            await Confirm(vIssue.Id, UserId);

                        return Result<Issue>.PrepareSuccess(vIssue);
                    }


                }
                else
                {
                    // var vIssueTitle = _context.IssueTitles.FirstOrDefault(x => x.Id == Int32.Parse(issueInfo.Title));

                    var vResult = new Issue
                    {
                        Id = issueInfo.Id,
                        //WorkArea = issueInfo.WorkArea,
                        DepartmentId = vUser.DepartmentId,
                        UserId = (UserId),
                        IssueNo = 0,
                        VersionNo = 0,
                        TitleId = issueInfo.TitleId,
                        SubtitleId = issueInfo.SubtitleId,
                        Summary = issueInfo.Summary,
                        Keywords = "",
                        Status = ActivityStatuses.Processing,
                        Deleted = false,
                        IssueActivitiys = vIssueActivitiys.ToList(),
                        IssueNotes = vIssueNote.ToList(),
                        IssuePreconditions = vPrecondition.ToList(),
                        IssueRelevantDepartmants = vRevelantDepartment.ToList(),
                        IssueRoles = vIssueRole.ToList(),
                        IssueAttachments = vAttachment.ToList(),

                    };

                    await _context.Issues.AddAsync(vResult);

                    await _context.SaveChangesAsync();
                    if (vAttachment.Any())
                    {
                        string tempPath = @"d:\Resources\temp";
                        string filePath = @"d:\Resources\Files";

                        if (!Directory.Exists(filePath))
                            Directory.CreateDirectory(filePath);

                        foreach (var file in vAttachment)
                        {
                            var tempFilePath = Path.Combine(tempPath, file.UniqueName);
                            var newFilePath = Path.Combine(filePath, file.UniqueName);

                            //if (!File.Exists(tempFilePath))
                            //    continue;

                            //if (File.Exists(newFilePath))
                            //    continue;

                            File.Move(tempFilePath, newFilePath);
                        }
                    }
                    if (issueInfo.IsSaveWithConfirm)
                        await Confirm(vResult.Id, UserId);
                    return Result<Issue>.PrepareSuccess(vResult);
                }

            }
            catch (Exception vEx)
            {
                _logger.LogError(vEx, "Add Issue Error");
                return Result<Issue>.PrepareFailure($"Add Issue Error. {vEx.Message}");
            }
        }

        public async Task<Result<Issue>> DeleteIssue(int id)
        {
            try
            {
                var vIssue = _context.Issues
                    .Include(x => x.IssueAttachments)
                    .FirstOrDefault(x => x.Id == id);

                if (vIssue == null)
                {
                    return Result<Issue>.PrepareFailure($"{id}'li Issue Bulunamadı.");
                }
                vIssue.Deleted = true;
                vIssue.IssueAttachments.ForEach(x =>
                {
                    x.Deleted = true;
                });
                await _context.SaveChangesAsync();

                return Result<Issue>.PrepareSuccess(vIssue);

            }
            catch (Exception vEx)
            {
                _logger.LogError(vEx, "Delete Issue Error");
                return Result<Issue>.PrepareFailure(vEx.Message);

            }
        }
        private List<IssueActivitiyDetail> GetIssueActivitiyDetails(IssueActivitiy activity, IssueActivitiyDetail parentNode, List<IssueActivitiyDetailInfo> nodes)
        {
            List<IssueActivitiyDetail> details = new List<IssueActivitiyDetail>();
            foreach (var item in nodes)
            {
                var detail = new IssueActivitiyDetail
                {
                    LineNo = item.LineNo,
                    Definition = item.Definition,
                    RoleId = (byte)item.RoleId,
                    Medium = item.Medium,
                    Explanation = item.Explanation,
                    Parent = parentNode,
                    IssueActivitiy = activity
                };

                if (item.IssueActivityDetailInfos != null)
                {
                    if (item.IssueActivityDetailInfos.Count > 0)
                        detail.IssueActivitiyDetails = GetIssueActivitiyDetails(activity, detail, item.IssueActivityDetailInfos);
                }

                details.Add(detail);
            }

            return details;
        }


        private List<IssueActivitiyDetail> GetIssueActivitiyDetails(IssueActivitiy activity, IssueActivitiyDetail parentNode, List<IssueActivitiyDetail> nodes)
        {
            List<IssueActivitiyDetail> details = new List<IssueActivitiyDetail>();
            foreach (var item in nodes)
            {
                var detail = new IssueActivitiyDetail
                {
                    LineNo = item.LineNo,
                    Definition = item.Definition,
                    RoleId = (byte)item.RoleId,
                    Medium = item.Medium,
                    Explanation = item.Explanation,
                    Parent = parentNode,
                    IssueActivitiy = activity
                };

                if (item.IssueActivitiyDetails != null)
                {
                    if (item.IssueActivitiyDetails.Count > 0)
                        detail.IssueActivitiyDetails = GetIssueActivitiyDetails(activity, detail, item.IssueActivitiyDetails);
                }

                details.Add(detail);
            }

            return details;
        }
    }
}