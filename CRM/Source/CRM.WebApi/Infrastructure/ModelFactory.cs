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
        private CRMDatabaseEntities db = new CRMDatabaseEntities();
        //public ContactResponseModel CreateContactResponseModel(ContactRequestModel crm)
        //{
        //    return new ContactResponseModel
        //    {
        //        FullName = crm.FullName,
        //        CompanyName = crm.CompanyName,
        //        Position = crm.Position,
        //        Country = crm.Country,
        //        Email = crm.Email,
        //        Guid = Guid.NewGuid(),
        //        DateInserted = DateTime.Now,
        //        MailingLists = new List<string>()
        //    };
        //}
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
                DateInserted = c.DateInserted,
                MailingLists = c.MailingLists.Select(x => x.MailingListName).ToList()
            };
        }

        //public Contact CreateContact(ContactResponseModel cresm)
        //{
        //    return new Contact
        //    {
        //        FullName = cresm.FullName,
        //        CompanyName = cresm.CompanyName,
        //        Position = cresm.Position,
        //        Country = cresm.Country,
        //        Email = cresm.Country,
        //        Guid = cresm.Guid,
        //        DateInserted = cresm.DateInserted,
        //        MailingLists = new List<MailingList>()
        //    };
        //}

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
            //var list = new List<MailingList>();
            //using (db)
            //{
            //    foreach (var mailingListName in creqm.MailingLists)
            //    {
            //        list.AddRange(db.MailingLists.Where(mailingList => mailingList.MailingListName == mailingListName));
            //    }
            //}
            //contact.MailingLists = list;
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
        public MailingListResponseModel CreateMailingListResponseModel(MailingListRequestModel mlrm)
        {
            return new MailingListResponseModel
            {
                MailingListName = mlrm.MailingListName,
                Contacts = new List<ContactResponseModel>()
            };
        }
        public MailingList CreateMailingList(MailingListRequestModel mlrm)
        {
            return new MailingList
            {
                MailingListName = mlrm.MailingListName,
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