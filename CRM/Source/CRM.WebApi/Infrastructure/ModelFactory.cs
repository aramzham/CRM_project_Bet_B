using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRM.EntityFramework;
using CRM.WebApi.Models;

namespace CRM.WebApi.Infrastructure
{
    public class ModelFactory
    {
        private static CRMDatabaseEntities db = new CRMDatabaseEntities();
        public static ContactResponseModel CreateContactResponseModel(ContactRequestModel crm)
        {
            return new ContactResponseModel
            {
                FullName = crm.FullName,
                CompanyName = crm.CompanyName,
                Position = crm.Position,
                Country = crm.Country,
                Email = crm.Email,
                Guid = Guid.NewGuid(),
                DateInserted = DateTime.Now,
                MailingLists = new List<string>()
            };
        }
        public static ContactResponseModel CreateContactResponseModel(Contact c)
        {
            return new ContactResponseModel
            {
                FullName = c.FullName,
                CompanyName = c.CompanyName,
                Position = c.Position,
                Country = c.Country,
                Email = c.Email,
                Guid = c.Guid,
                DateInserted = c.DateInserted,
                MailingLists = c.MailingLists.Select(x => x.MailingListName).ToList()
            };
        }
        public static Contact CreateContact(ContactRequestModel creqm)
        {
            var contact = new Contact
            {
                FullName = creqm.FullName,
                CompanyName = creqm.CompanyName,
                Position = creqm.Position,
                Country = creqm.Country,
                Email = creqm.Email,
                Guid = Guid.NewGuid(),
                DateInserted = DateTime.Now,
            };
            var list = new List<MailingList>();
            using (db)
            {
                foreach (var mailingListName in creqm.MailingLists)
                {
                    list.AddRange(db.MailingLists.Where(mailingList => mailingList.MailingListName == mailingListName));
                }
            }
            contact.MailingLists = list;
            return contact;
        }
    }
}