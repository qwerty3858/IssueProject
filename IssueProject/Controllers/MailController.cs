using EmailService;
using IssueProject.Entity.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace IssueProject.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MailController : ControllerBase
    {
        private readonly IEmailSender _emailSender;

        private _2Mes_ConceptualContext _context;

        public MailController(_2Mes_ConceptualContext context, IEmailSender emailSender)
        {
            _context = context;
            _emailSender = emailSender;

            //RecurringJob.AddOrUpdate("SendMailJob", () => JobMethod(), "* * * * *", TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time"));
        }

        [HttpGet]
        public async Task MailMessage()
        {
            var vIssueConfirms = _context.IssueConfirms.Where(x => x.Status == 0).ToList();

            foreach (var vIssueConfirm in vIssueConfirms)
            {
                var vUser = _context.Issues.Include(x => x.User).FirstOrDefault(x => x.UserId == vIssueConfirm.UserId);
                var message = new Message(new string[] { vUser.User.EmailAddress }, "Test email ", vUser.User.FullName +
                         " kişi tarafından Bilgilendirme Maili gönderilmiştir."
                         , null);
                //vIssueConfirm.Status = ConfirmStatuses.MailGonderildiBeklemede;
                await _emailSender.SendEmailAsync(message);
            }
        }
        [HttpPost]
        public async Task MailMessageWithFile()
        {
            var files = Request.Form.Files.Any() ? Request.Form.Files : new FormFileCollection();

            var message = new Message(new string[] { "poyraz.celal97@gmail.com" }, "Test mail with Attachments", "This is the content from our mail with attachments.", files);
            await _emailSender.SendEmailAsync(message);
        }
    }
}
