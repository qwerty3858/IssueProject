using EmailService;
using IssueProject.Entity.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

                var message = new Message(new string[] { "poyraz.celal97@gmail.com" }, "Test email ", "   Bilgilendirme Maili gönderilmiştir."
                         , null);
                //vIssueConfirm.Status = ConfirmStatuses.MailGonderildiBeklemede;
                MessageIsSend checkMail = new MessageIsSend();
                await _emailSender.SendEmailAsync(message, checkMail);
         
        }
        [HttpPost]
        public async Task MailMessageWithFile()
        {
            var files = Request.Form.Files.Any() ? Request.Form.Files : new FormFileCollection();

            var message = new Message(new string[] { "poyraz.celal97@gmail.com" }, "Test mail with Attachments", "This is the content from our mail with attachments.", files);
            MessageIsSend checkMail = new MessageIsSend();
            await _emailSender.SendEmailAsync(message, checkMail);
        }


    }
}
