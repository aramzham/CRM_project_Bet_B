using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using CRM.EntityFramework;

namespace CRM.WebApi.Infrastructure
{
    public class MailManager
    {
        private CRMDatabaseEntities db = new CRMDatabaseEntities();
        public void SendMailTo(List<string> emailAddresses)
        {
            var msg = new MailMessage("aram.j90@gmail.com", string.Join(",", emailAddresses), "Test", "Hi, from Bet b team!");
            msg.IsBodyHtml = true;
            var sc = new SmtpClient("smtp.gmail.com", 587);
            sc.UseDefaultCredentials = false;
            var netCredential = new NetworkCredential("aram.j90@gmail.com", "smtp587x");//("vanhakobyan1996@gmail.com", "Van606580!!");
            sc.Credentials = netCredential;
            sc.EnableSsl = true;
            sc.Send(msg);
        }

        public async Task<List<string>> GetMails(string[] guids)
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
            return emails;
        }

        public void SendMailTo(List<Contact> emailAddresses, int templateId)
        {
            //var messageText = 
            var msg = new MailMessage("aram.j90@gmail.com", string.Join(",", emailAddresses), "Test", "Hi, from Bet b team!");
            msg.IsBodyHtml = true;
            var sc = new SmtpClient("smtp.gmail.com", 587);
            sc.UseDefaultCredentials = false;
            var netCredential = new NetworkCredential("aram.j90@gmail.com", "smtp587x");//("vanhakobyan1996@gmail.com", "Van606580!!");
            sc.Credentials = netCredential;
            sc.EnableSsl = true;
            sc.Send(msg);
        }

        private async Task<string> GetTemplateText(int templateId)
        {
            var template = await db.Templates.FindAsync(templateId);
            return File.ReadAllText(template.PathToFile);
        }

        private string ReplacePlaceholders(string text, Contact contact)
        {
            return
                text.Replace("[FullName]", contact.FullName)
                    .Replace("[CompanyName]", contact.CompanyName)
                    .Replace("[Position]", contact.Position)
                    .Replace("[Country]", contact.Country)
                    .Replace("[Email]", contact.Email);
        }

        private async Task<List<Contact>> GetRecepients(string[] guids)
        {
            var recepients = new List<Contact>();
            Contact contact;
            using (var db = new CRMDatabaseEntities())
            {
                foreach (var guid in guids)
                {
                    contact = await db.Contacts.Where(x => x.Guid.ToString() == guid).FirstOrDefaultAsync();
                    if (contact != null) recepients.Add(contact);
                }
            }
            return recepients;
        }
    }
}