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
using CRM.WebApi.Models;

namespace CRM.WebApi.Infrastructure
{
    //TODO: async problem!
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
        public void SendMailTo(string emailAddress, int templateId)
        {
            var msg = new MailMessage("aram.j90@gmail.com", emailAddress, "Test", "Hi, from Bet b team!");
            msg.IsBodyHtml = true;
            var sc = new SmtpClient("smtp.gmail.com", 587);
            sc.UseDefaultCredentials = false;
            var netCredential = new NetworkCredential("aram.j90@gmail.com", "smtp587x");//("vanhakobyan1996@gmail.com", "Van606580!!");
            sc.Credentials = netCredential;
            sc.EnableSsl = true;
            sc.Send(msg);
        }

        //public async Task<List<string>> GetMails(string[] guids)
        //{
        //    var emails = new List<string>();
        //    Contact contact;
        //    using (var db = new CRMDatabaseEntities())
        //    {
        //        foreach (var guid in guids)
        //        {
        //            contact = await db.Contacts.Where(x => x.Guid.ToString() == guid).FirstOrDefaultAsync();
        //            if (contact != null) emails.Add(contact.Email);
        //        }
        //    }
        //    return emails;
        //}
        public async Task<bool> SendMailToMailingList(int mailingListId, int templateId)
        {
            try
            {
                if (!(await db.Templates.Select(x => x.Id).ToListAsync()).Contains(templateId)) return false;
                var mailingList = await db.MailingLists.FirstOrDefaultAsync(x => x.ID == mailingListId);
                if (mailingList == null) return false;

                var contacts = mailingList.Contacts.ToList();
                if (contacts.Count == 0) return false;
                SendMailToListOfContacts(contacts, templateId);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void SendMailToListOfContacts(List<Contact> contacts, int templateId)
        {
            foreach (var contact in contacts)
            {
                SendMailToSingleContact(contact, templateId);
            }
        }

        public async void SendMailToSingleContact(Contact contact, int templateId)
        {
            //var templateText = GetTemplate    Text(templateId).Result;
            //var messageText = ReplacePlaceholders(templateText, contact);
            //var messageText = await GetMessageText(templateId, contact);
            var messageText = GetMessageText(templateId, contact);
            var msg = new MailMessage("aram.j90@gmail.com", contact.Email, "Test", messageText);
            msg.IsBodyHtml = true;
            var sc = new SmtpClient("smtp.gmail.com", 587);
            sc.UseDefaultCredentials = false;
            var netCredential = new NetworkCredential("aram.j90@gmail.com", "smtp587x");//("vanhakobyan1996@gmail.com", "Van606580!!");
            sc.Credentials = netCredential;
            sc.EnableSsl = true;
            sc.Send(msg);
        }

        //private async Task<string> GetMessageText(int templateId, Contact contact)
        private string GetMessageText(int templateId, Contact contact)
        {
            //var template = await db.Templates.FindAsync(templateId);
            var template = db.Templates.Find(templateId);
            var path = System.Web.HttpContext.Current?.Request.MapPath(template.PathToFile);
            var templateText = File.ReadAllText(path);
            return
                templateText.Replace("[FullName]", contact.FullName)
                    .Replace("[CompanyName]", contact.CompanyName)
                    .Replace("[Position]", contact.Position)
                    .Replace("[Country]", contact.Country)
                    .Replace("[Email]", contact.Email);
        }

        public async Task<List<Contact>> GetRecipients(string[] guids)
        {
            var recipients = new List<Contact>();
            Contact contact;
            using (var db = new CRMDatabaseEntities())
            {
                foreach (var guid in guids)
                {
                    contact = await db.Contacts.Where(x => x.Guid.ToString() == guid).FirstOrDefaultAsync();
                    if (contact != null) recipients.Add(contact);
                }
            }
            return recipients;
        }
    }
}