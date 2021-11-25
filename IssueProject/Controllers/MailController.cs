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
            string myvar = "<table width = '100%' cellspacing = '0' cellpadding = '0' ><tr> <td>";
            myvar += "<table cellspacing = '0' cellpadding = '0'><tr><td style = 'border-radius: 2px; ' bgcolor = '#ED2939'>";
            myvar += $"<a href =\" http://localhost:8080/Viewer/1\"";
            myvar += " target = '_blank'";
            myvar += "style = 'padding: 8px 12px; border: 1px solid #ED2939; border-radius: 2px; font-family: Helvetica, Arial, sans-serif;font-size: 14px; color: #FFFFFF; text-decoration: none;font-weight:bold;display: inline-block;'>";
            myvar += "Maili Görüntüle </a></td></tr></table></td></tr></table>";

            var message = new Message(new string[] { "poyraz.celal97@gmail.com" }, "Test email async", myvar, null);
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
