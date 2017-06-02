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
        public ContactResponseModel CreateContactResponseModel(Contact c)
        {
            return new ContactResponseModel
            {
                FullName = c.FullName,
                CompanyName = c.CompanyName,
                Position = c.Position,
                Country = c.Country,
                Email = c.Email,
                Guid = c.Guid,
                MailingLists = c.MailingLists.Select(x => x.MailingListName).ToList()
            };
        }

        public Contact CreateContact(ContactRequestModel creqm)
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
                MailingLists = new List<MailingList>()
            };
            return contact;
        }

        public MailingListResponseModel CreateMailingListResponseModel(MailingList ml)
        {
            return new MailingListResponseModel
            {
                MailingListId = ml.ID,
                MailingListName = ml.MailingListName,
                Contacts = ml.Contacts.Select(CreateContactResponseModel).ToList()
            };
        }
        public MailingList CreateMailingList(string mailingListName)
        {
            return new MailingList
            {
                MailingListName = mailingListName,
                Contacts = new List<Contact>()
            };
        }

        public TemplateResponseModel CreateTemplateResponseModel(Template t)
        {
            return new TemplateResponseModel
            {
                Id = t.Id,
                TemplateName = t.TemplateName
            };
        }
    }
}