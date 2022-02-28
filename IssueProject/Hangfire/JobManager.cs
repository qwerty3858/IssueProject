using EmailService;
using Hangfire;
using IssueProject.Common;
using IssueProject.Entity.Context;
using IssueProject.Hangfire.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace IssueProject.Hangfire
{
    public class JobManager : IJob
    {
        private IEmailSender _emailSender;

        private _2Mes_ConceptualContext _context;
        IRecurringJobManager recurringJobManager;

        public JobManager(_2Mes_ConceptualContext context, IEmailSender emailSender)
        {
            _context = context;
            _emailSender = emailSender;

        }
        public static void Create(IServiceProvider provider)
        {
            var _context = provider.GetService(typeof(_2Mes_ConceptualContext));
            var _emailSender = provider.GetService(typeof(IEmailSender));

            JobManager jobManager = new JobManager((_2Mes_ConceptualContext)_context, (IEmailSender)_emailSender);
            jobManager.recurringJobManager = new RecurringJobManager();

            jobManager.recurringJobManager.AddOrUpdate(
            "SendMail",
            () => jobManager.JobMethod(),
            //*/59 */7 * * *
            "*/1 * * * *", TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time")
            );
        }
        public async Task JobMethod()
        {
            var vIssueConfirms = await _context.IssueConfirms
                .Include(x=>x.Issue)
                .ThenInclude(x=>x.User)
                .Include(x=>x.Issue)
                .ThenInclude(x=>x.Title) 
                .Where(x => x.Status == ConfirmStatuses.MailSendWaiting
            || x.Status == ConfirmStatuses.Rejected || x.Status == ConfirmStatuses.Commited).ToListAsync();
            if (vIssueConfirms != null)
            {
                foreach (var vIssueConfirm in vIssueConfirms)
                {
                    var vUser = await _context.Users.FirstOrDefaultAsync(x => x.Id == vIssueConfirm.UserId);
                    if (vUser != null)
                    {
                        vIssueConfirm.MailTime = DateTime.Now;
                        if (vIssueConfirm.Status == ConfirmStatuses.Rejected && vIssueConfirm.IsRejectSend)
                        {
                            string content = $"{vUser.FullName} tarafından Talep Reddedilme Maili gönderilmiştir.<br>";
                            content += $"<br> <span style='color:red'>Konu : </span> {vIssueConfirm.Issue.Title.Subject}<br>";
                            content += $"<br> <span style='color:red'>Kısa Açıklama : </span> {vIssueConfirm.Issue.Summary}<br>";
                            content += $"<br> <span style='color:red'>Red Sebebi : </span> {vIssueConfirm.Description}<br>";
                            
                            var vUserForRejected = await _context.Issues.Include(x => x.User).FirstOrDefaultAsync(x => x.Id == vIssueConfirm.IssueId);
                            var messageReject = new Message(new string[] { vUserForRejected.User.EmailAddress }, "Talep Reddi ", content, null);
                            //vIssueConfirm.Status = ConfirmStatuses.MailSent;
                            MessageIsSend checkMail = new MessageIsSend();
                            await _emailSender.SendEmailAsync(messageReject, checkMail);
                            if (checkMail.IsSend)
                            {
                                vIssueConfirm.IsRejectSend = false;
                                _context.SaveChanges();
                            }

                        }
                        else if (vIssueConfirm.Status == ConfirmStatuses.Commited && vIssueConfirm.IsCommited)
                        {
                            string content = $"{vUser.FullName} tarafından Talep Onaylama Bilgilendirme Maili gönderilmiştir.<br>";
                            content += $"<br> <span style='color:red'>Konu : </span> {vIssueConfirm.Issue.Title.Subject}<br>";
                            content += $"<br> <span style='color:red'>Kısa Açıklama : </span> {vIssueConfirm.Issue.Summary}<br>"; 
                            var vUserForCommited = await _context.Issues.Include(x => x.User).FirstOrDefaultAsync(x => x.Id == vIssueConfirm.IssueId);
                            var message = new Message(new string[] { vUserForCommited.User.EmailAddress }, "Talep Onay ", content , null);
                            // vIssueConfirm.Status = ConfirmStatuses.MailSent;

                            MessageIsSend checkMail = new MessageIsSend();
                            await _emailSender.SendEmailAsync(message, checkMail);
                            if (checkMail.IsSend)
                            {
                                vIssueConfirm.IsCommited = false;
                                _context.SaveChanges();
                            }

                        }
                        else if (vIssueConfirm.Status == ConfirmStatuses.MailSendWaiting)
                        {
                            string content = $"{vIssueConfirm.Issue.User.FullName} tarafından Talep Bilgilendirme Maili gönderilmiştir.<br>";
                            content += $"<br> <span style='color:red'>Konu : </span> {vIssueConfirm.Issue.Title.Subject}<br>";
                            content += $"<br> <span style='color:red'>Kısa Açıklama : </span> {vIssueConfirm.Issue.Summary}<br>";
                           
                            var message = new Message(new string[] { vUser.EmailAddress }, "Talep Formu ",  content  , null);
                            MessageIsSend checkMail = new MessageIsSend();
                            await _emailSender.SendEmailAsync(message, checkMail);
                            if (checkMail.IsSend)
                            {
                                vIssueConfirm.Status = ConfirmStatuses.MailSent;
                                _context.SaveChanges();
                            }

                        }
                    }
                }
            }
        }
    }
}
