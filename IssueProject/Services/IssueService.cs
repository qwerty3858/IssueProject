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
        public async Task<Result<List<TitleInfo>>> GetTitleInfo()
        {
            try
            {
                var vResult = await _context.IssueTitles.Select(x => new TitleInfo
                {
                    Id = x.Id,
                    Subject = x.Subject
                }).ToListAsync();
                if (vResult == null)
                {
                    _logger.LogInformation("İstenilen Title için veri bulunamadı. ");
                    return Result<List<TitleInfo>>.PrepareFailure("İstenilen Title veri bulunamadı.");
                }
                return Result<List<TitleInfo>>.PrepareSuccess(vResult);
            }
            catch (Exception vEx)
            {
                _logger.LogInformation($"Title Info List Error: {vEx.Message}");
                return Result<List<TitleInfo>>.PrepareFailure(vEx.Message);
            }
        }
        public async Task<Result<IssueConfirm>> GetRejectReason(int IssueId)
        {
            try
            {
                var vResult = await _context.IssueConfirms.FirstOrDefaultAsync(x => x.IssueId == IssueId && x.Status == ConfirmStatuses.Rejected);

                if (vResult == null)
                {
                    _logger.LogInformation("İstenilen Red Sebebi bulunamadı. ");
                    return Result<IssueConfirm>.PrepareFailure("İstenilen Red Sebebi bulunamadı.");
                }
                return Result<IssueConfirm>.PrepareSuccess(vResult);
            }
            catch (Exception vEx)
            {
                _logger.LogInformation($"Reject Reason Info Error: {vEx.Message}");
                return Result<IssueConfirm>.PrepareFailure(vEx.Message);
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
                _logger.LogInformation($"SubTitle Info List Error: {vEx.Message}");
                return Result<List<SubTitleInfo>>.PrepareFailure(vEx.Message);
            }
        }
        public async Task<Result<List<VersionInfo>>> GetVersionInfoList(int IssueId)
        {
            try
            {
                var vIssue = await _context.Issues.FirstOrDefaultAsync(x => x.Id == IssueId);

                var vVersionInfos = await _context.Issues.Where(x => x.IssueNo == vIssue.IssueNo).ToListAsync();

                var VersionInfo = vVersionInfos.Select(x => new VersionInfo
                {
                    Id = x.Id,
                    VersionNo = x.VersionNo,
                    IssueNo = x.IssueNo
                });
               

                //List<IssueInfo> issueInfo = _mapper.Map<List<IssueInfo>>(vVersonInfos);
                return Result<List<VersionInfo>>.PrepareSuccess(VersionInfo.ToList());
            }
            catch (Exception vEx)
            {
                _logger.LogInformation($"Version Info List Error: {vEx.Message}");
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
                _logger.LogInformation($"Version Info List Error: {vEx.Message}");
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

                            if (vParentItem.IssueActivitiyDetails.All(x => x.Id != vSubItem.Id))
                                vParentItem.IssueActivitiyDetails.Add(vSubItem);
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
                {
                    _logger.LogInformation("İstenilen sorguya ait veri bulunamadı. ");
                    return Result<IssueInfo>.PrepareFailure("İstenilen sorguya ait veri bulunamadı.");
                }
                return Result<IssueInfo>.PrepareSuccess(issueInfo);
            }
            catch (Exception vEx)
            {
                _logger.LogInformation($"Selected List Error: {vEx.Message}");
                return Result<IssueInfo>.PrepareFailure(vEx.Message);
            }
        }
        public async Task<Result> Upload(IFormFile file)
        {
            try
            {
                // List<IssueAttachment> vIssueAttachment;
                var folderName = Path.Combine("Resources", "Images");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                if (file.Length > 0)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    var fullPath = Path.Combine(pathToSave, fileName);
                    var dbPath = Path.Combine(folderName, fileName);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    return Result.PrepareSuccess("Dosya eklendi");
                }

                _logger.LogInformation("Dosya Yüklenemedi. ");
                return Result<IssueAttachment>.PrepareFailure("İstenilen sorguya ait veri bulunamadı.");

            }
            catch (Exception ex)
            {
                _logger.LogInformation($" File Upload Error: {ex.Message}");
                return Result<IssueAttachment>.PrepareFailure(ex.Message);
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
                                WorkArea = x.WorkArea,
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
                _logger.LogInformation($"Super Admin Issue List Error: {vEx.Message}");
                return Result<List<IssueSummary>>.PrepareFailure(vEx.Message);
            }
        }

        public async Task<Result<List<IssueSummary>>> GetListByUserId(int userId)
        {
            try
            {
                var vIssues = await _context.Issues
                    .Include(x => x.IssueRelevantDepartmants)
                    .Include(x => x.Department)
                    .Include(x => x.User)
                    .ThenInclude(x => x.Role)
                    .Where(x => x.UserId == userId && x.Deleted == false)
                    .Select(x => new IssueSummary
                    {
                        Id = x.Id,
                        WorkArea = x.WorkArea,
                        DepartmentName = x.Department.Definition,
                        FullName = x.User.FullName,
                        RoleName = x.User.Role.Definition,
                        Status = ((int)x.Status),
                        Title = x.Title
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
                _logger.LogInformation($"Private Issue List Error: {vEx.Message}");
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
                    .Where(x => x.Issue.Status != ActivityStatuses.Rejected && x.UserId == vUser.Id
                                && (x.Status == ConfirmStatuses.MailSent || x.Status == ConfirmStatuses.MailSendWaiting))
                    .Select(x => new IssueSummary
                    {
                        Id = x.Issue.Id,
                        WorkArea = x.Issue.WorkArea,
                        DepartmentName = x.Issue.Department.Definition,
                        FullName = x.Issue.User.FullName,
                        RoleName = x.Issue.User.Role.Definition,
                        Status = (int)x.Issue.Status,
                        Title = x.Issue.Title

                    }).ToListAsync();

                if (vIssues.Count == 0)
                {
                    return Result<List<IssueSummary>>.PrepareFailure("Size Gelen Issue Bilgisi Bulunamadı.");
                }
                return Result<List<IssueSummary>>.PrepareSuccess(vIssues);
            }
            catch (Exception vEx)
            {
                _logger.LogInformation($"Come To Me Issue List Error: {vEx.Message}");
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
                    if (Issue.IssueConfirms.Any(y => y.UserId == vUserId && y.IsConfirm))
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
                    }
                }

                Issue.Status = ActivityStatuses.Rejected;

                await _context.SaveChangesAsync();

                return Result<Issue>.PrepareSuccess(Issue);
            }
            catch (Exception vEx)
            {
                _logger.LogInformation($"Reject Error: {vEx.Message}");
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
                    if (Issue.IssueConfirms.Any(y => y.UserId == UserId && y.IsConfirm))
                        return Result<Issue>.PrepareFailure("Onaylamak istediğiniz talep için izniniz bulunmamaktadır!");

                    //if (Issue.IssueRelevantDepartmants.All(x => x.Department.Users.All(y => y.Id != UserId)))
                    //    return Result<Issue>.PrepareFailure("Onaylamak istediğiniz madde için izniniz bulunmamaktadır!");
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
                                if (vUser.IsKeyUser)
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
                                                if (vUser.IsKeyUser)
                                                {
                                                    _context.IssueConfirms.Add(new IssueConfirm
                                                    {
                                                        IssueId = IssueId,
                                                        VersionNo = Issue.VersionNo,
                                                        DepartmentId = vDepartment.DepartmentId,
                                                        UserId = vUser.Id,
                                                        Status = ConfirmStatuses.MailSendWaiting,
                                                        CreateTime = DateTime.Now,
                                                        IsConfirm = false
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
                                            if (vUser.IsManager)
                                            {
                                                _context.IssueConfirms.Add(new IssueConfirm
                                                {
                                                    IssueId = IssueId,
                                                    VersionNo = Issue.VersionNo,
                                                    DepartmentId = Issue.DepartmentId,
                                                    UserId = vUser.Id,
                                                    Status = ConfirmStatuses.MailSendWaiting,
                                                    CreateTime = DateTime.Now,
                                                    IsConfirm = false
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
                                        Issue.Status = ActivityStatuses.ManagerCommitted;
                                }
                            }
                            break;
                        }
                    /*
                case ActivityStatuses.Rejected:
                    {
                        byte versionValue = 0;
                        foreach (var vConfirm in Issue.IssueConfirms.ToList())
                        {
                            if (vConfirm.UserId == UserId)
                            {
                                vConfirm.Status = ConfirmStatuses.Commited;
                                vConfirm.IsConfirm = true;
                                vConfirm.SubmitTime = DateTime.Now;
                                versionValue = vConfirm.VersionNo;
                                foreach (var vUser in vBimDepartment.Users)
                                {
                                    if (vUser.IsKeyUser)
                                    {
                                        _context.IssueConfirms.Add(new IssueConfirm
                                        {
                                            IssueId = IssueId,
                                            VersionNo = vConfirm.VersionNo,
                                            DepartmentId = vUser.DepartmentId,
                                            UserId = vUser.Id,
                                            Status = ConfirmStatuses.MailSendWaiting,
                                            CreateTime = DateTime.Now,
                                            SubmitTime = DateTime.Now,
                                            IsConfirm = false,
                                        });
                                    }

                                }
                                var vRevelantDepartment = issueInfo.IssueRelevantDepartmentInfos.Select(x => new IssueRelevantDepartmant
                                {

                                    DepartmentId = x.DepartmentId
                                });
                                var vPrecondition = issueInfo.IssuePreconditionInfos.Select(x => new IssuePrecondition
                                {
                                    LineNo = x.LineNo,
                                    Explanation = x.Explanation
                                });
                                var vIssueNote = issueInfo.IssueNoteInfos.Select(x => new IssueNote
                                {
                                    LineNo = x.LineNo,
                                    Explanation = x.Explanation
                                });
                                List<IssueActivitiy> vIssueActivitiys = new List<IssueActivitiy>();
                                foreach (var vIssueActivityInfo in issueInfo.IssueActivitiyInfos)
                                {
                                    var vIssueActivity = new IssueActivitiy
                                    {
                                        Type = vIssueActivityInfo.Type,
                                        SubActivityNo = vIssueActivityInfo.SubActivityNo,
                                        SubActivityTitle = vIssueActivityInfo.SubActivityTitle
                                    };

                                    vIssueActivity.IssueActivitiyDetails = GetIssueActivitiyDetails(vIssueActivity, null, vIssueActivityInfo.IssueActivityDetailInfos);

                                    vIssueActivitiys.Add(vIssueActivity);
                                }
                                var vIssueRole = issueInfo.IssueRoleInfos.Select(x => new IssueRole
                                {
                                    RoleId = (byte)(x.RoleId)
                                });
                                var vAttachment = issueInfo.IssueAttachmentInfos.Select(x => new IssueAttachment
                                {
                                    Deleted = false,
                                    FileName = x.FileName,
                                    UniqueName = Guid.NewGuid().ToString()
                                });
                                var vResult = new Issue
                                {
                                    //Id = issueInfo.Id,
                                    WorkArea = issueInfo.WorkArea,
                                    DepartmentId = Issue.DepartmentId,
                                    UserId = (UserId),
                                    IssueNo = (short)IssueId,
                                    VersionNo = versionValue,
                                    Title = issueInfo.Title,
                                    Subtitle = issueInfo.Subtitle,
                                    Summary = issueInfo.Summary,
                                    Keywords = "",
                                    Status = ActivityStatuses.ITWaiting,
                                    Deleted = false,
                                    IssueActivitiys = vIssueActivitiys.ToList(),
                                    IssueNotes = vIssueNote.ToList(),
                                    IssuePreconditions = vPrecondition.ToList(),
                                    IssueRelevantDepartmants = vRevelantDepartment.ToList(),
                                    //IssueRoles = vIssueRole.ToList(),
                                    IssueAttachments = vAttachment.ToList(),

                                };

                                await _context.Issues.AddAsync(vResult);

                            }

                        }
                        break;
                    }
                    */
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
                _logger.LogInformation($"Confirm Error: {vEx.Message}");
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
                    UniqueName = Guid.NewGuid().ToString(),
                    
                });

                if (issueInfo.Id > 0)
                {
                    var vIssue = _context.Issues
                            .Include(x => x.IssueActivitiys)
                            .ThenInclude(x => x.IssueActivitiyDetails)
                            .Include(x => x.IssuePreconditions)
                            //.ThenInclude(x => x.Issue)
                            .Include(x => x.IssueNotes)
                            .Include(x => x.IssueRelevantDepartmants)
                            // .ThenInclude(x => x.Department)
                            .Include(x => x.IssueAttachments)
                            .Include(x => x.IssueRoles)
                        //.ThenInclude(x => x.Role)
                        .FirstOrDefault(x => x.Id == issueInfo.Id);

                    if (vIssue == null)
                    {
                        return Result<Issue>.PrepareFailure("İstenilen sorguya ait veri bulunamadı.");
                    }
                    vIssue.WorkArea = issueInfo.WorkArea;
                    //vIssue.DepartmentId = vUser.DepartmentId;
                    //vIssue.UserId = (UserId);
                    vIssue.IssueNo = (short)issueInfo.Id;
                    vIssue.VersionNo = 0;
                    vIssue.Title = issueInfo.Title;
                    vIssue.Subtitle = issueInfo.Subtitle;
                    vIssue.Summary = issueInfo.Summary;
                    vIssue.Status = ActivityStatuses.Processing;
                    vIssue.IssueActivitiys = vIssueActivitiys.ToList();
                     vIssue.IssueNotes = vIssueNote.ToList();
                    vIssue.IssuePreconditions = vPrecondition.ToList();
                    vIssue.IssueRelevantDepartmants = vRevelantDepartment.ToList();
                    vIssue.IssueRoles = vIssueRole.ToList();
                    vIssue.IssueAttachments = vAttachment.ToList();
                    await _context.SaveChangesAsync();
                    if (issueInfo.Status == ActivityStatuses.Rejected && issueInfo.IsSaveWithConfirm)
                    {
                        vIssue.Id = 0;
                        await _context.Issues.AddAsync(vIssue);
                        await _context.SaveChangesAsync();
                        await Confirm(vIssue.Id, UserId);
                      
                    }
                    
                    return Result<Issue>.PrepareSuccess(vIssue);
                }
                else
                {
                    var vResult = new Issue
                    {
                        Id = issueInfo.Id,
                        WorkArea = issueInfo.WorkArea,
                        DepartmentId = vUser.DepartmentId,
                        UserId = (UserId),
                        IssueNo = 0,
                        VersionNo = 0,
                        Title = issueInfo.Title,
                        Subtitle = issueInfo.Subtitle,
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
                    if (issueInfo.IsSaveWithConfirm)
                        await Confirm(vResult.Id, UserId);
                    return Result<Issue>.PrepareSuccess(vResult);
                }

            }
            catch (Exception vEx)
            {
                _logger.LogInformation($"Add Issue Error.{vEx.InnerException}");
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
                _logger.LogInformation($"Delete Issue Error.{vEx.InnerException}");
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