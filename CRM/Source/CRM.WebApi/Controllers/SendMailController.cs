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
        public async Task<IHttpActionResult> Post([FromBody] string[] guids)
        {
            return await SendEmailByGuids(guids);
        }

        private void SendMailTo(List<string> emailAddresses)
        {
            var msg = new MailMessage("aram.j90@gmail.com", string.Join(",", emailAddresses), "Test", "Hi, how're you doin', bro?");
            msg.IsBodyHtml = true;
            var sc = new SmtpClient("smtp.gmail.com", 587);
            sc.UseDefaultCredentials = false;
            var netCredential = new NetworkCredential("aram.j90@gmail.com", "smtp587x");//("vanhakobyan1996@gmail.com", "Van606580!!");
            sc.Credentials = netCredential;
            sc.EnableSsl = true;
            sc.Send(msg);
        }

        private async Task<IHttpActionResult> SendEmailByGuids(string[] guids)
        {
            var emails = new List<string>();
            Contact contact;
            using (var db = new CRMDatabaseEntities())
            {
                foreach (var guid in guids)
                {
                    contact = await db.Contacts.Where(x => x.Guid.ToString() == guid).FirstOrDefaultAsync();
                    if (contact != null) emails.Add(contact.Email); 
                }
            }
            if (emails.Count == 0) return BadRequest("No emails were found");
            else
            {
                SendMailTo(emails);
                return Ok();
            }
        }
    }
}
