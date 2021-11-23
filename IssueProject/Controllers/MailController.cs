using EmailService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IssueProject.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MailController : ControllerBase
    {
        private readonly IEmailSender _emailSender;

        public MailController(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        [HttpGet]
        public async Task MailMessage()
        {
            var message = new Message(new string[] { "poyraz.celal97@gmail.com" }, "Test email async", "This is the content from our async email.", null);
            await _emailSender.SendEmailAsync(message);
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
