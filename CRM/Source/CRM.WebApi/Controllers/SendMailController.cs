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
    //TODO: send mail to mailing list
    public class SendMailController : ApiController
    {
        private MailManager mailManager = new MailManager();
        //private ModelFactory modelFactory = new ModelFactory();
        // POST: api/SendMail
        //public async Task<IHttpActionResult> Post([FromBody] string[] guids)
        //{
        //    var emails = await mailManager.GetMails(guids);
        //    if (emails.Count == 0) return BadRequest("No emails were found");
        //    else
        //    {
        //        mailManager.SendMailTo(emails);
        //        return Ok();
        //    }
        //}
        // POST: api/SendMail
        [Route("api/SendMail/{templateId}")]
        [HttpPost]
        public async Task<IHttpActionResult> Post([FromBody] string[] guids, [FromUri]int templateId)
        {
            var contacts = await mailManager.GetRecipients(guids);
            if (contacts.Count == 0) return BadRequest("No recipients were found");
            else
            {
                mailManager.SendMailToListOfContacts(contacts, templateId);
                return Ok();
            }
        }

        // POST: api/SendMail
        [Route("api/SendMail/{mailingListId}/{templateId}")]
        [HttpPost]
        public async Task<IHttpActionResult> Post(int mailingListId, int templateId)
        {
            if (!await mailManager.SendMailToMailingList(mailingListId, templateId)) return BadRequest("Either there's no such mailing list or mailing list contains no contact");
            return Ok();
        }
    }
}
