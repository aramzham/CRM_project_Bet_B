using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web.Http;
using CRM.EntityFramework;
using CRM.WebApi.Infrastructure;

namespace CRM.WebApi.Controllers
{
    public class SendMailController : ApiController
    {
        private MailManager mailManager = new MailManager();
        // POST: api/SendMail
        public async Task<IHttpActionResult> Post([FromBody] string[] guids)
        {
            var emails = await mailManager.GetMails(guids);
            if (emails.Count == 0) return BadRequest("No emails were found");
            else
            {
                mailManager.SendMailTo(emails);
                return Ok();
            }
        }
        // POST: api/SendMail
        public async Task<IHttpActionResult> Post([FromBody] string[] guids, [FromUri]int templateId)
        {
            var emails = await mailManager.GetMails(guids);
            if (emails.Count == 0) return BadRequest("No emails were found");
            else
            {
                mailManager.SendMailTo(emails, templateId);
                return Ok();
            }
        }
    }
}
