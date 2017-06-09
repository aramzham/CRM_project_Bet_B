using System.Threading.Tasks;
using System.Web.Http;
using CRM.WebApi.Infrastructure;
using CRM.WebApi.Infrastructure.ApplicationManagers;

namespace CRM.WebApi.Controllers
{
    [NotImplExceptionFilter]
    public class SendMailController : ApiController
    {
        private MailManager mailManager = new MailManager();
        private ApplicationManager appManager = new ApplicationManager();

        // POST: api/SendMail
        [HttpPost]
        [Route("api/SendMail/{templateId}")]
        public async Task<IHttpActionResult> Post([FromBody] string[] guids, int templateId)
        {
            if (!await appManager.TemplateExists(templateId)) return BadRequest("Template doesn't exist");

            if (guids == null || guids.Length == 0) return BadRequest("No recipients were found");

            var contacts = await mailManager.GetRecipients(guids);
            if (contacts.Count == 0) return BadRequest("No recipients were found");

            await mailManager.SendMailToListOfContacts(contacts, templateId);
            return Ok();
        }

        // POST: api/SendMail
        [Route("api/SendMail/{mailingListId}/{templateId}")]
        [HttpPost]
        public async Task<IHttpActionResult> Post(int mailingListId, int templateId)
        {
            if (!await appManager.TemplateExists(templateId)) return BadRequest("Template doesn't exist");

            if (!await mailManager.SendMailToMailingList(mailingListId, templateId)) return BadRequest("Either there's no such mailing list or mailing list contains no contact");

            return Ok();
        }
    }
}
