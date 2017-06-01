﻿using System;
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
    //TODO: open a new gmail test mail
    public class MailManager
    {
        private CRMDatabaseEntities db = new CRMDatabaseEntities();

        public async Task<bool> SendMailToMailingList(int mailingListId, int templateId)
        {
            try
            {
                if (!(await db.Templates.Select(x => x.Id).ToListAsync()).Contains(templateId)) return false;
                var mailingList = await db.MailingLists.FirstOrDefaultAsync(x => x.ID == mailingListId);
                if (mailingList == null) return false;

                var contacts = mailingList.Contacts.ToList();
                if (contacts.Count == 0) return false;
                await SendMailToListOfContacts(contacts, templateId);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task SendMailToListOfContacts(List<Contact> contacts, int templateId)
        {
            foreach (var contact in contacts)
            {
                await SendMailToSingleContact(contact, templateId);
            }
        }

        //public async Task SendMailToSingleContact(Contact contact, int templateId)
        //{
        //    //var templateText = GetTemplate    Text(templateId).Result;
        //    //var messageText = ReplacePlaceholders(templateText, contact);
        //    //var messageText = await GetMessageText(templateId, contact);
        //    var messageText = await GetMessageText(templateId, contact);
        //    var msg = new MailMessage("aram.j90@gmail.com", contact.Email, $"Test for {contact.FullName}", messageText);
        //    msg.IsBodyHtml = true;
        //    var sc = new SmtpClient("smtp.gmail.com", 587);
        //    sc.UseDefaultCredentials = false;
        //    var netCredential = new NetworkCredential("aram.j90@gmail.com", "smtp587x");
        //    sc.Credentials = netCredential;
        //    sc.EnableSsl = true;
        //    sc.Send(msg);
        //}

        public async Task SendMailToSingleContact(Contact contact, int templateId)
        {
            var messageText = await GetMessageText(templateId, contact);
            var msg = new MailMessage { Body = messageText, IsBodyHtml = true, Subject = $"Test for {contact.FullName}" };
            msg.To.Add(contact.Email);
            var sc = new SmtpClient();
            sc.Send(msg);
        }

        private async Task<string> GetMessageText(int templateId, Contact contact)
        //private string GetMessageText(int templateId, Contact contact)
        {
            var template = await db.Templates.FindAsync(templateId);
            //var template = db.Templates.Find(templateId);
            var path = HttpContext.Current?.Request.MapPath(template?.PathToFile);
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