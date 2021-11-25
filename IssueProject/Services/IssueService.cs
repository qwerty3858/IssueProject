using EmailService;
using IssueProject.Common;
using IssueProject.Entity;
using IssueProject.Entity.Context;
using IssueProject.Enums.Issue;
using IssueProject.Models.Issue;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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

        public async Task<Result<List<Issue>>> SelectedListById(int id)
        {
            try
            {
                //var vLoginUserId = User.GetSubject<int>();
                var vIssues = await _context.Issues
                            .Include(x=>x.IssueActivitiys)
                            .Include(x=>x.IssuePreconditions)
                            .Include(x=>x.Department)
                            .Include(x=>x.User)
                            .Include(x=>x.IssueNotes)
                            .Include(x=>x.IssueRelevantDepartmants)
                            .Where(x=>x.Id==id)
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
                _logger.LogInformation($"Auth Login Error: {vEx.Message}");
                return Result<List<Issue>>.PrepareFailure(vEx.Message);
            }
        }

        public async Task<Result<List<IssueSummary>>> GetList()
        {
            try
            {
                //var vLoginUserId = User.GetSubject<int>();
                var vIssues = await _context.Issues

                            .Select(x => new IssueSummary
                            {
                                Id = x.Id,
                                WorkArea = x.WorkArea,
                                DepartmentName = x.Department.Definition,
                                FullName = x.User.FullName,
                                RoleName = x.User.Role.Definition,
                                Status = ((int)x.Status)

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
                _logger.LogInformation($"Auth Login Error: {vEx.Message}");
                return Result<List<IssueSummary>>.PrepareFailure(vEx.Message);
            }
        }

        public async Task<Result<List<IssueSummary>>> GetListByUserId(string userId)
        {
            try
            {
            //var vLoginUserId = User.GetSubject<int>();
            var vIssues =await _context.Issues.Where(x => x.UserId.ToString() == userId)
                
                        .Select(x => new IssueSummary
                        {
                            Id=x.Id,
                            WorkArea=x.WorkArea,
                            DepartmentName=x.Department.Definition,
                            FullName=x.User.FullName,
                            RoleName=x.User.Role.Definition,
                            Status= ((int)x.Status)

                        }).ToListAsync();
                if(vIssues == null)
                {
                    _logger.LogInformation("İstenilen sorguya ait veri bulunamadı. ");
                    return Result<List<IssueSummary>>.PrepareFailure("İstenilen sorguya ait veri bulunamadı.");
                }
            return Result<List<IssueSummary>>.PrepareSuccess(vIssues);
            }catch(Exception vEx)
            {
                _logger.LogInformation($"Auth Login Error: {vEx.Message}");
                return Result<List<IssueSummary>>.PrepareFailure(vEx.Message);
            }
        }


        public async Task<Result<Issue>> AddIssue(IssueInfo issueInfo)
        {
            try
            {
                var vRevelantDepartment = issueInfo.IssueRelevantDepartmantInfos.Select(x => new IssueRelevantDepartmant
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

                var vIssueActivity = issueInfo.IssueActivitiyInfos.Select(x => new IssueActivitiy
                {
                    Type = x.Type,
                    SubActivityNo = x.SubActivityNo,
                    SubActivityTitle = x.SubActivityTitle,
                    IssueActivitiyDetails = x.IssueActivitiyDetailInfos.Select(x => new IssueActivitiyDetail
                    {
                        LineNo = x.LineNo,
                        Definition = x.Definition,
                        RoleId = x.RoleId,
                        Medium = x.Medium,
                        Explanation = x.Explanation
                    }).ToList()


                });
                var vConfirm = issueInfo.IssueConfirmInfos.Select(x => new IssueConfirm
                {
                    VersionNo = x.VersionNo,
                    DepartmentId = x.DepartmentId,
                    UserId = x.UserId,
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

                var vResult = new Issue
                {
                    Id = issueInfo.Id,
                    WorkArea = issueInfo.WorkArea,
                    DepartmentId = issueInfo.DepartmentId,
                    UserId = issueInfo.UserId,
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

                string myvar = $"<a href =\" http://localhost:8080/Viewer/{ vResult.Id}\"";
                myvar += " target = '_blank'";
                myvar += "style = 'padding: 8px 12px; border: 1px solid #ED2939; border-radius: 2px; font-family: Helvetica, Arial, sans-serif;font-size: 14px; color: #FFFFFF; text-decoration: none;font-weight:bold;display: inline-block;'>";
                myvar += "Maili Görüntüle </a>";

                var message = new Message(new string[] { "poyraz.celal97@gmail.com" }, "Test email async", myvar, null);
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