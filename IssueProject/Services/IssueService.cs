using EmailService;
using IssueProject.Common;
using IssueProject.Entity;
using IssueProject.Entity.Context;
using IssueProject.Enums.Issue;
using IssueProject.Models.Issue;
using IssueProject.Models.IssueComfirm;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace IssueProject.Services
{
    public class IssueService
    {
        ILogger<IssueService> _logger;

        IEmailSender _emailSender;

        _2Mes_ConceptualContext _context;
        private IHostingEnvironment Environment;
        private IEnumerable<IssueConfirm> list;
        public IssueService(_2Mes_ConceptualContext context, IEmailSender emailSender, ILogger<IssueService> logger, IHostingEnvironment _environment)
        {
            _context = context;
            _emailSender = emailSender;
            _logger = logger;
            Environment = _environment;

        }

        public async Task<Result<List<Issue>>> SelectedListById(int id)
        {
            try
            { 
                var vIssues = await _context.Issues
                            .Include(x => x.IssueActivitiys)
                            .Include(x => x.IssuePreconditions)
                            .Include(x => x.Department)
                            .Include(x => x.User)
                            .Include(x => x.IssueNotes)
                            .Include(x => x.IssueRelevantDepartmants)
                            .Where(x => x.Id == id)
                           .ToListAsync();
                if (vIssues == null)
                {
                    _logger.LogInformation("İstenilen sorguya ait veri bulunamadı. ");
                    return Result<List<Issue>>.PrepareFailure("İstenilen sorguya ait veri bulunamadı.");
                }
                return Result<List<Issue>>.PrepareSuccess(vIssues);
            }
            catch (Exception vEx)
            {
                _logger.LogInformation($"Selected List Error: {vEx.Message}");
                return Result<List<Issue>>.PrepareFailure(vEx.Message);
            }
        }

        public async Task<Result<List<IssueSummary>>> GetList()
        {
            try
            { 
                var vIssues = await _context.Issues.Include(x => x.IssueRelevantDepartmants).Where(x=>x.Deleted==false)

                            .Select(x => new IssueSummary
                            {
                                Id = x.Id,
                                WorkArea = x.WorkArea,
                                DepartmentName = x.Department.Definition,
                                FullName = x.User.FullName,
                                RoleName = x.User.Role.Definition,
                                Status = ((int)x.Status),
                                Deparment = new Models.IssueRelevantDepartMent.IssueRelevantDepartmentInfo
                                {
                                    DepartmentId = x.DepartmentId
                                }

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
                _logger.LogInformation($"Public List Error: {vEx.Message}");
                return Result<List<IssueSummary>>.PrepareFailure(vEx.Message);
            }
        }

        public async Task<Result<List<IssueSummary>>> GetListByUserId(string userId)
        {
            try
            { 
                var vIssues = await _context.Issues.Include(x => x.IssueRelevantDepartmants).Where(x => x.UserId.ToString() == userId && x.Deleted == false)

                            .Select(x => new IssueSummary
                            {
                                Id = x.Id,
                                WorkArea = x.WorkArea,
                                DepartmentName = x.Department.Definition,
                                FullName = x.User.FullName,
                                RoleName = x.User.Role.Definition,
                                Status = ((int)x.Status),
                                Deparment = new Models.IssueRelevantDepartMent.IssueRelevantDepartmentInfo
                                {
                                    DepartmentId = x.DepartmentId
                                }

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
                _logger.LogInformation($"Private List Error: {vEx.Message}");
                return Result<List<IssueSummary>>.PrepareFailure(vEx.Message);
            }
        }

        public async Task<Result> Reject(int departmentId,string vUserId,string description)
        {
            try
            {
                var vIssues = await _context.Issues.Include(x => x.User).FirstOrDefaultAsync(x => x.DepartmentId == departmentId && x.UserId.ToString() == vUserId);

                if (vIssues == null)
                {
                    _logger.LogInformation("İstenilen sorguya ait veri bulunamadı. ");
                    return Result<Issue>.PrepareFailure("İstenilen sorguya ait veri bulunamadı.");
                }
                vIssues.IssueConfirms.Select(x => new IssueConfirmInfo
                {
                    Status = Enums.Confirm.ConfirmStatus.Reddedildi,
                    Description = description
                });
                vIssues.Status = Status.RedYapilmayacak;
               
                vIssues.IssueConfirms.Select(x => new IssueConfirmInfo
                {
                    CreateTime = DateTime.Now,
                    MailTime = DateTime.Now,
                    SubmitTime = DateTime.Now,
                    DepartmentId = departmentId,
                    UserId = x.UserId
                }).ToList();
                var message = new Message(new string[] { vIssues.User.EmailAddress }, "Talep Reddi ", vIssues.User.FullName +
                " kişi tarafından Talep Reddedilme Maili gönderilmiştir.\n Açıklama : "+ description
                , null);
                await _emailSender.SendEmailAsync(message);

                if (message == null)
                {
                    _logger.LogInformation("Mail Gönderilemedi.");
                    return Result<Issue>.PrepareFailure("Mail Gönderilemedi.");
                }
                await _context.SaveChangesAsync();
                return Result<Issue>.PrepareSuccess(vIssues);
            }
            catch (Exception vEx)
            {
                _logger.LogInformation($"Reject Error: {vEx.Message}");
                return Result<List<IssueSummary>>.PrepareFailure(vEx.Message);
            }
        }
        public async Task<Result<Issue>> Confirm(int departmentId,string vUserId)
        {
            try
            {
               
                var vIssues = await _context.Issues.Include(x=>x.User).Include( x=>x.IssueConfirms).FirstOrDefaultAsync(x => x.DepartmentId == departmentId && x.UserId.ToString() == vUserId);

                if (vIssues == null)
                {
                    _logger.LogInformation("İstenilen sorguya ait veri bulunamadı. ");
                    return Result<Issue>.PrepareFailure("İstenilen sorguya ait veri bulunamadı.");
                }
                switch (vIssues.Status)
                {
                    case Status.BimOnayBekleme:
                        {
                            foreach(var item in vIssues.IssueConfirms)
                            {
                                item.Status = Enums.Confirm.ConfirmStatus.MailGonderildiBeklemede;
                            }
                            await _context.SaveChangesAsync();
                            //  vIssues.IssueConfirms.Select(x => new IssueConfirm
                            //{
                            //    Status = Enums.Confirm.ConfirmStatus.MailGonderildiBeklemede
                            //}).ToList();
                            //await _context.SaveChangesAsync();
                            vIssues.Status = Status.BimOnay;
                            break;
                        }
                        case Status.BimOnay:
                        {
                            foreach (var item in vIssues.IssueConfirms)
                            {
                                item.Status = Enums.Confirm.ConfirmStatus.MailGonderildiBeklemede;
                            }
                            vIssues.Status = Status.DepartmanOnay;
                            break;
                        }
                    case Status.DepartmanOnay:
                        {
                            foreach (var item in vIssues.IssueConfirms)
                            {
                                item.Status = Enums.Confirm.ConfirmStatus.MailGonderildiBeklemede;
                            }
                             
                             
                            vIssues.Status = Status.YazanDepartmanAmirOnay;
                            break;
                        }
                    case Status.YazanDepartmanAmirOnay:
                        {
                            foreach (var item in vIssues.IssueConfirms)
                            {
                                item.Status = Enums.Confirm.ConfirmStatus.Onaylandi;
                            }

                         
                            break;
                        }
                    case Status.Onaylandi:
                        {
                            foreach (var item in vIssues.IssueConfirms)
                            {
                                item.Status = Enums.Confirm.ConfirmStatus.MailGonderildiBeklemede;
                            }
                             
                            break;
                        }
                    default:
                        {
                            foreach (var item in vIssues.IssueConfirms)
                            {
                                item.Status = Enums.Confirm.ConfirmStatus.MailGonderildiBeklemede;
                            }
                             
                            vIssues.Status = Status.Kilitli;
                            break;
                        }
                }
                vIssues.IssueConfirms.Select(x => new IssueConfirm
                {
                    CreateTime = DateTime.Now,
                    MailTime = DateTime.Now,
                    SubmitTime = DateTime.Now,
                    DepartmentId = departmentId,
                    UserId = x.UserId
                }).ToList();

                var message = new Message(new string[] { vIssues.User.EmailAddress }, "Test email ", vIssues.User.FullName +
                " kişi tarafından Bilgilendirme Maili gönderilmiştir."
                , null);
                await _emailSender.SendEmailAsync(message);

                if (message == null)
                {
                    _logger.LogInformation("Mail Gönderilemedi.");
                    return Result<Issue>.PrepareFailure("Mail Gönderilemedi.");
                }

                await _context.SaveChangesAsync();
            
                return Result<Issue>.PrepareSuccess(vIssues);
            }
            catch (Exception vEx)
            {
                _logger.LogInformation($"Confirm Error: {vEx.Message}");
                return Result<Issue>.PrepareFailure(vEx.Message);
            }

        } 

        public async Task<Result<Issue>> AddIssue(IssueInfo issueInfo, string UserId)
        {
            try
            { 
                var vUser = _context.Users.FirstOrDefault(x => x.Id.ToString() == UserId && x.Deleted ==false);
                if (vUser == null)
                {
                    return Result<Issue>.PrepareFailure("User Bulunamadı.");
                }
                var vSuperAdmin = _context.Users.Include(x => x.Role).FirstOrDefault(x => x.Role.Definition == "SuperAdmin" && x.Deleted == false);
                if (vSuperAdmin == null)
                {
                    return Result<Issue>.PrepareFailure("Admin Bulunamadı.");
                }

                var vRevelantDepartment = issueInfo.IssueRelevantDepartmantInfos.Select(x => new IssueRelevantDepartmant
                {

                    DepartmentId = vUser.DepartmentId
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
                
                var vIssueActivity = issueInfo.IssueActivitiyInfos.Select(x => new IssueActivitiy
                {
                    Type = x.Type,
                    SubActivityNo = x.SubActivityNo,
                    SubActivityTitle = x.SubActivityTitle,
                    IssueActivitiyDetails = x.IssueActivitiyDetailInfos.Select(x => new IssueActivitiyDetail
                    {
                        LineNo = x.LineNo,
                        Definition = x.Definition,
                        RoleId = vUser.RoleId,
                        Medium = x.Medium,
                        Explanation = x.Explanation
                    }).ToList()
                     
                });
                var vConfirm = issueInfo.IssueConfirmInfos.Select(x => new IssueConfirm
                {
                    VersionNo = x.VersionNo,
                    DepartmentId = x.DepartmentId,
                    UserId = Int32.Parse(UserId),
                    Status = Enums.Confirm.ConfirmStatus.MailGonderildiBeklemede,
                    Description = x.Description,
                    CreateTime = DateTime.Now,
                    MailTime = DateTime.Now,
                    SubmitTime = DateTime.Now
                });

                var vIssueRole = issueInfo.IssueRoleInfos.Select(x => new IssueRole
                {
                    RoleId = x.RoleId
                });

                if (issueInfo.IssueAttachmentInfos.Any())
                {
                    string wwwPath = this.Environment.WebRootPath;
                    string contentPath = this.Environment.ContentRootPath;

                    string path = Path.Combine(this.Environment.WebRootPath, "Uploads");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    List<string> uploadedFiles = new List<string>();
                    foreach (IFormFile postedFile in issueInfo.IssueAttachmentInfos.Select(x=>x.postedFiles))
                    {
                        string fileName = Path.GetFileName(postedFile.FileName);
                        using (FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
                        {
                            postedFile.CopyTo(stream);
                            uploadedFiles.Add(fileName);
                        }
                    }
                }

                var vResult = new Issue
                {
                    Id = issueInfo.Id,
                    WorkArea = issueInfo.WorkArea,
                    DepartmentId = vUser.DepartmentId,
                    UserId = Int32.Parse(UserId),
                    IssueNo = issueInfo.IssueNo,
                    VersionNo = issueInfo.VersionNo,
                    Title = issueInfo.Title,
                    Subtitle = issueInfo.Subtitle,
                    Summary = issueInfo.Summary,
                    Keywords = issueInfo.Keywords,
                    Status = Status.BimOnayBekleme,
                    Deleted = false,
                    IssueActivitiys = vIssueActivity.ToList(),
                    IssueNotes = vIssueNote.ToList(),
                    IssuePreconditions = vPrecondition.ToList(),
                    IssueRelevantDepartmants = vRevelantDepartment.ToList(),
                    IssueRoles = vIssueRole.ToList(),
                    IssueConfirms = vConfirm.ToList()
                };

                await _context.Issues.AddAsync(vResult);
                await _context.SaveChangesAsync();

                var message = new Message(new string[] { vSuperAdmin.EmailAddress }, "Test email ", vUser.FullName +
                    " kişi tarafından Bilgilendirme Maili gönderilmiştir."
                    , null);
                await _emailSender.SendEmailAsync(message);

                if (message == null)
                {
                    _logger.LogInformation("Mail Gönderilemedi.");
                    return Result<Issue>.PrepareFailure("Mail Gönderilemedi.");
                }

                return Result<Issue>.PrepareSuccess(vResult);

            }
            catch (Exception vEx)
            {
                _logger.LogInformation($"Add Issue Error.{vEx.InnerException}");
                return Result<Issue>.PrepareFailure($"Add Issue Error. {vEx.Message}");

            }
        }
    }
}