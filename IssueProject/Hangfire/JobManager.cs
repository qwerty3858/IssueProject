using EmailService;
using Hangfire;
using IssueProject.Common;
using IssueProject.Entity.Context;
using IssueProject.Hangfire.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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

            JobManager jobManager = new JobManager((_2Mes_ConceptualContext)_context,(IEmailSender)_emailSender);
            jobManager.recurringJobManager = new RecurringJobManager();
            
            jobManager.recurringJobManager.AddOrUpdate(
            "SendMail",
            () => jobManager.JobMethod(),
            "*/5 * * * *", TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time")
            );
        }
        public async Task JobMethod()
        {
            var vIssueConfirms =await _context.IssueConfirms.Where(x => x.Status == ConfirmStatuses.MailSendWaiting /*|| x.Status == ConfirmStatuses.Reddedildi*/).ToListAsync();
            if(vIssueConfirms != null)
            {
                foreach (var vIssueConfirm in vIssueConfirms)
                {
                    var vUser =await _context.Users.FirstOrDefaultAsync(x => x.Id == vIssueConfirm.UserId);
                    if (vUser != null)
                    {
                        //if(vIssueConfirm.Status == ConfirmStatuses.MailGonderilmedi)
                        //{
                        vIssueConfirm.MailTime = DateTime.Now;
                        var message = new Message(new string[] { vUser.EmailAddress }, "Test email ", vUser.FullName +
                            " kişi tarafından Talep Bilgilendirme Maili gönderilmiştir."
                            , null);
                        vIssueConfirm.Status = ConfirmStatuses.MailSent;
                        _context.SaveChanges();
                        await _emailSender.SendEmailAsync(message);
                        //}
                        //else
                        //{
                        //    vIssueConfirm.MailTime = DateTime.Now;
                        //    var messageReject = new Message(new string[] { vUser.EmailAddress }, "Talep Reddi ", vUser.FullName +
                        // " kişi tarafından Talep Reddedilme Maili gönderilmiştir.\n Açıklama : " + vIssueConfirm.Description
                        // , null);
                        //    vIssueConfirm.Status = ConfirmStatuses.MailGonderildiBeklemede;
                        //    _context.SaveChanges();
                        //    await _emailSender.SendEmailAsync(messageReject);
                        //}


                    }



                }
            }
            
        }
    }
}
