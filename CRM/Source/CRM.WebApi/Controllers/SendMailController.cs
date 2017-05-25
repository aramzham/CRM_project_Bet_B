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

namespace CRM.WebApi.Controllers
{
    public class SendMailController : ApiController
    {
        // POST: api/SendMail
        public async Task<IHttpActionResult> Post([FromBody] Guid[] guids)
        {
            var listOfEmails= new List<string>();
            foreach (var guid in guids)
            {
                listOfEmails.Add(await FindEmailByGuid(guid));
            }
            return Ok();
        }

        private void SendMailTo(List<string> emailAddresses)
        {
            var msg = new MailMessage("aram.j90@gmail.com", string.Join(",", emailAddresses), "Test", "Hi, how're you doin', bro?");
            msg.IsBodyHtml = true;
            var sc = new SmtpClient("smtp.gmail.com", 587);
            sc.UseDefaultCredentials = false;
            var netCredential = new NetworkCredential("aram.j90@gmail.com", "krlovice");
            sc.Credentials = netCredential;
            sc.EnableSsl = true;
            sc.Send(msg);
        }

        private async Task<string> FindEmailByGuid(Guid guid)
        {
            using (var db = new CRMDatabaseEntities())
            {
                var contact = await db.Contacts.FirstOrDefaultAsync(x => x.Guid == guid);
                return contact.Email;
            }
        }
    }
}
