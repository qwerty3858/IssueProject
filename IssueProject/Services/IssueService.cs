using EmailService;
using IssueProject.Common;
using IssueProject.Entity;
using IssueProject.Entity.Context;
using IssueProject.Enums.Issue;
using IssueProject.Models.Issue;
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

namespace IssueProject.Services
{
    public class IssueService
    {
        ILogger<IssueService> _logger;

        IEmailSender _emailSender;

        _2Mes_ConceptualContext _context;
        public IssueService(_2Mes_ConceptualContext context, IEmailSender emailSender, ILogger<IssueService> logger)
        {
            _context = context;
            _emailSender = emailSender;
            _logger = logger;

        }

        public async Task<Result<List<Issue>>> SelectedIssueById(int id)
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
                      await  file.CopyToAsync(stream);
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
                var vIssues = await _context.Issues.Include(x => x.IssueRelevantDepartmants).Where(x => x.Deleted == false)

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
                _logger.LogInformation($"Super Admin Issue List Error: {vEx.Message}");
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
                _logger.LogInformation($"Private Issue List Error: {vEx.Message}");
                return Result<List<IssueSummary>>.PrepareFailure(vEx.Message);
            }
        }
        public async Task<Result<List<IssueSummary>>> GetListComeToMeIssues(string userId)
        {
            try
            {
                
                var vUser = await _context.Users.FirstOrDefaultAsync(x => x.Id.ToString() == userId && x.Deleted == false);
                if (vUser == null)
                {
                    _logger.LogInformation("İstenilen sorguya ait veri bulunamadı. ");
                    return Result<List<IssueSummary>>.PrepareFailure("İstenilen sorguya ait veri bulunamadı.");
                }

                var vIssues = await _context.Issues.Include(x=>x.IssueRelevantDepartmants).Where(x => x.DepartmentId == vUser.DepartmentId)
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
        public async Task<Result> Reject(int departmentId, string vUserId, string description)
        {
            try
            {
                var vIssues = await _context.Issues.Include(x => x.User).FirstOrDefaultAsync(x => x.DepartmentId == departmentId && x.UserId.ToString() == vUserId);

                if (vIssues == null)
                {
                    _logger.LogInformation("İstenilen sorguya ait veri bulunamadı. ");
                    return Result<Issue>.PrepareFailure("İstenilen sorguya ait veri bulunamadı.");
                }
                vIssues.IssueConfirms.ForEach(x => new IssueConfirmInfo
                {
                    Status = Enums.Confirm.ConfirmStatus.Reddedildi,
                    Description = description,
                    DepartmentId = departmentId,
                    UserId = x.UserId,
                    SubmitTime = DateTime.Now,

                });
                vIssues.Status = Status.RedYapilmayacak;

               
                var message = new Message(new string[] { vIssues.User.EmailAddress }, "Talep Reddi ", vIssues.User.FullName +
                " kişi tarafından Talep Reddedilme Maili gönderilmiştir.\n Açıklama : " + description
                , null);
                await _emailSender.SendEmailAsync(message);

                if (message == null)
                {
                    _logger.LogInformation("Mail Gönderilemedi.");
                    return Result<Issue>.PrepareFailure("Mail Gönderilemedi.");
                }
                vIssues.IssueConfirms.ForEach(x => new IssueConfirmInfo
                {
                    MailTime = DateTime.Now,
                });
                await _context.SaveChangesAsync();
                return Result<Issue>.PrepareSuccess(vIssues);
            }
            catch (Exception vEx)
            {
                _logger.LogInformation($"Reject Error: {vEx.Message}");
                return Result<List<IssueSummary>>.PrepareFailure(vEx.Message);
            }
        }

        public async Task<Result<Issue>> Confirm(int departmentId, string vUserId)
        {
            try
            {

                var vIssues = await _context.Issues.Include(x => x.User).Include(x => x.IssueConfirms).FirstOrDefaultAsync(x => x.DepartmentId == departmentId && x.UserId.ToString() == vUserId);

                if (vIssues == null)
                {
                    _logger.LogInformation("İstenilen sorguya ait veri bulunamadı. ");
                    return Result<Issue>.PrepareFailure("İstenilen sorguya ait veri bulunamadı.");
                }
                switch (vIssues.Status)
                {
                    case Status.BimOnayBekleme:
                        {
                            vIssues.IssueConfirms.ForEach(x =>
                            x.Status = Enums.Confirm.ConfirmStatus.MailGonderildiBeklemede
                            );

                            vIssues.Status = Status.BimOnay;
                            break;
                        }
                    case Status.BimOnay:
                        {
                            vIssues.IssueConfirms.ForEach(x =>
                            x.Status = Enums.Confirm.ConfirmStatus.MailGonderildiBeklemede
                            );
                            vIssues.Status = Status.DepartmanOnay;
                            break;
                        }
                    case Status.DepartmanOnay:
                        {
                            vIssues.IssueConfirms.ForEach(x =>
                           x.Status = Enums.Confirm.ConfirmStatus.MailGonderildiBeklemede
                           );

                            vIssues.Status = Status.YazanDepartmanAmirOnay;
                            break;
                        }
                    case Status.YazanDepartmanAmirOnay:
                        {
                            vIssues.IssueConfirms.ForEach(x =>
                            x.Status = Enums.Confirm.ConfirmStatus.MailGonderildiBeklemede
                            );
                            break;
                        }
                    case Status.Onaylandi:
                        {
                            vIssues.IssueConfirms.ForEach(x =>
                            x.Status = Enums.Confirm.ConfirmStatus.Onaylandi
                            );

                            break;
                        }
                    default:
                        {
                            vIssues.IssueConfirms.ForEach(x =>
                            x.Status = Enums.Confirm.ConfirmStatus.MailGonderilmedi
                            );

                            vIssues.Status = Status.Kilitli;
                            break;
                        }
                }
                vIssues.IssueConfirms.ForEach(x =>
                {
                    x.SubmitTime = DateTime.Now;
                    x.DepartmentId = departmentId;
                });

                var message = new Message(new string[] { vIssues.User.EmailAddress }, "Test email ", vIssues.User.FullName +
                " kişi tarafından Bilgilendirme Maili gönderilmiştir."
                , null);
                await _emailSender.SendEmailAsync(message);

                if (message == null)
                {
                    _logger.LogInformation("Mail Gönderilemedi.");
                    return Result<Issue>.PrepareFailure("Mail Gönderilemedi.");
                }
                vIssues.IssueConfirms.ForEach(x =>
                {
                    x.MailTime = DateTime.Now;
                });
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
                var vUser = _context.Users.FirstOrDefault(x => x.Id.ToString() == UserId && x.Deleted == false);
                if (vUser == null)
                {
                    return Result<Issue>.PrepareFailure("User Bulunamadı.");
                }
                var vSuperAdmin = _context.Users.Include(x => x.Role).FirstOrDefault(x => x.Role.Definition == "SuperAdmin" && x.Deleted == false);
                if (vSuperAdmin == null)
                {
                    return Result<Issue>.PrepareFailure("Admin Bulunamadı.");
                }

                var vRevelantDepartment = issueInfo.IssueRelevantDepartmentInfos.Select(x => new IssueRelevantDepartmant
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

                var vConfirm = new List<IssueConfirm>
                {
                    new IssueConfirm()
                    {
                    DepartmentId = vUser.DepartmentId,
                    UserId = Int32.Parse(UserId),
                    Status = Enums.Confirm.ConfirmStatus.MailGonderildiBeklemede,
                    Description="",
                    CreateTime = DateTime.Now,
                    MailTime = DateTime.Now,
                    SubmitTime = DateTime.Now,
                    }
                };
                var vIssueRole = issueInfo.IssueRoleInfos.Select(x => new IssueRole
                {
                    RoleId = (byte)(x.RoleId)
                });
                var vAttachment = issueInfo.IssueAttachmentInfos.Select(x => new IssueAttachment
                {
                    Deleted=false,
                    FileName=x.FileName,
                    UniqueName=Guid.NewGuid().ToString()
                });


                var vResult = new Issue
                {
                    //Id = issueInfo.Id,
                    WorkArea = short.Parse(issueInfo.WorkArea),
                    DepartmentId = vUser.DepartmentId,
                    UserId = Int32.Parse(UserId),
                    IssueNo = 1,
                    VersionNo = 1,
                    Title = issueInfo.Title,
                    Subtitle = issueInfo.Subtitle,
                    Summary = issueInfo.Summary,
                    Keywords = "",
                    Status = Status.BimOnayBekleme,
                    Deleted = false,
                    IssueActivitiys = vIssueActivity.ToList(),
                    IssueNotes = vIssueNote.ToList(),
                    IssuePreconditions = vPrecondition.ToList(),
                    IssueRelevantDepartmants = vRevelantDepartment.ToList(),
                    IssueRoles = vIssueRole.ToList(),
                    IssueConfirms = vConfirm,
                    IssueAttachments= vAttachment.ToList()

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

        
        public async Task<Result<Issue>> DeleteIssue(int id)
        {
            try
            {
                var vIssue = _context.Issues
                    .Include(x=>x.IssueAttachments)
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

}
}