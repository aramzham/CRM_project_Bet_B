using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Xml.Schema;
using CRM.EntityFramework;
using CRM.WebApi.Models;

namespace CRM.WebApi.Infrastructure
{
    public class ApplicationManager : IDisposable
    {
        private CRMDatabaseEntities db = new CRMDatabaseEntities();
        private ModelFactory modelFactory = new ModelFactory();
        #region Contacts methods
        public async Task<List<ContactResponseModel>> GetAllContacts()
        {
            var listOfContacts = await db.Contacts.ToListAsync();
            return listOfContacts.Select(modelFactory.CreateContactResponseModel).ToList();
        }
        #region By ID methods
        public async Task<Contact> GetContactById(int id)
        {
            return await db.Contacts.FindAsync(id);
        }
        public async Task<Contact> AddContact(Contact contact)
        {
            contact.Guid = Guid.NewGuid();
            contact.DateInserted = DateTime.Now;

            db.Contacts.Add(contact);
            await db.SaveChangesAsync();

            return contact;
        }
        public async Task<bool> UpdateContact(int id, Contact contact)
        {
            var contactToUpdate = await db.Contacts.FindAsync(id);
            if (contactToUpdate != null)
            {
                contactToUpdate.FullName = contact.FullName;
                contactToUpdate.CompanyName = contact.CompanyName;
                contactToUpdate.Position = contact.Position;
                contactToUpdate.Country = contact.Country;
                contactToUpdate.Email = contact.Email;
            }
            db.Entry(contactToUpdate).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await ContactExists(id)) return false;
                else throw;
            }
            return true;
        }
        public async Task<Contact> RemoveContact(int id)
        {
            var contact = await GetContactById(id);

            db.Contacts.Remove(contact);
            await db.SaveChangesAsync();

            return contact;
        }
        public async Task<bool> ContactExists(int id)
        {
            return await db.Contacts.CountAsync(e => e.ID == id) > 0;
        }
        #endregion
        public async Task<ContactResponseModel> GetContactByGuid(string guid)
        {
            return modelFactory.CreateContactResponseModel(await db.Contacts.Where(x => x.Guid.ToString() == guid).FirstOrDefaultAsync());
        }

        public async Task<List<Contact>> GetByPage(int start, int numberOfRows, bool @ascending)
        {
            if (ascending) return await db.Contacts.OrderBy(x => x.ID).Skip(start - 1).Take(numberOfRows).ToListAsync();
            else return await db.Contacts.OrderByDescending(x => x.ID).Skip(start - 1).Take(numberOfRows).ToListAsync();
        }


        public async Task<bool> UpdateContact(string guid, ContactRequestModel contact)
        {
            var contactToUpdate = await db.Contacts.FirstOrDefaultAsync(x => x.Guid.ToString() == guid);
            if (contactToUpdate != null)
            {
                contactToUpdate.FullName = contact.FullName;
                contactToUpdate.CompanyName = contact.CompanyName;
                contactToUpdate.Position = contact.Position;
                contactToUpdate.Country = contact.Country;
                contactToUpdate.Email = contact.Email;
                //contactToUpdate.
                db.Entry(contactToUpdate).State = EntityState.Modified;
            }

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await ContactExists(guid)) return false;
                else throw;
            }
            return true;
        }
        public async Task<Contact> AddContact(ContactRequestModel requestContact)
        {
            var contactToAdd = modelFactory.CreateContact(requestContact);
            db.Contacts.Add(contactToAdd);
            await db.SaveChangesAsync();

            return contactToAdd;
        }


        public async Task<ContactResponseModel> RemoveContact(string guid)
        {
            var contact = await db.Contacts.Where(x => x.Guid.ToString() == guid).FirstOrDefaultAsync();

            db.Contacts.Remove(contact);
            await db.SaveChangesAsync();

            return modelFactory.CreateContactResponseModel(contact);
        }

        public async Task<List<ContactResponseModel>> RemoveContactByGroup(string[] guids)
        {
            var listOfRemovedContacts = new List<ContactResponseModel>();
            foreach (var guid in guids)
            {
                listOfRemovedContacts.Add(await RemoveContact(guid));
            }
            return listOfRemovedContacts;
        }

        public async Task<bool> ContactExists(string guid)
        {
            return await db.Contacts.CountAsync(e => e.Guid.ToString() == guid) > 0;
        }

        public async Task<List<string>> GetAllEmails()
        {
            return await db.Contacts.Select(x => x.Email).ToListAsync();
        }
        #endregion
        #region Mailing lists methods
        public async Task<List<MailingListResponseModel>> GetAllMailingLists()
        {
            var mailingLists = await db.MailingLists.ToListAsync();
            return mailingLists.Select(x => modelFactory.CreateMailingListResponseModel(x)).ToList();
        }

        public async Task<MailingListResponseModel> GetMailingListById(int id)
        {
            var mailingList = await db.MailingLists.FirstOrDefaultAsync(x => x.ID == id);
            return mailingList == null ? null : modelFactory.CreateMailingListResponseModel(mailingList);
        }

        public async Task<MailingList> AddMailingList(MailingListRequestModel mlrm)
        {
            var mailingListToAdd = modelFactory.CreateMailingList(mlrm);
            db.MailingLists.Add(mailingListToAdd);
            await db.SaveChangesAsync();

            return mailingListToAdd;
        }

        public async Task<bool> UpdateMailingList(int id, List<ContactRequestModel> contacts)
        {
            var mailingList = await db.MailingLists.FindAsync(id);
            Contact contact;
            foreach (var contactRequestModel in contacts)
            {
                contact = await db.Contacts.FirstOrDefaultAsync(x => x.Guid == contactRequestModel.Guid);
                if (mailingList.Contacts.Contains(contact)) mailingList.Contacts.Remove(contact);
                else mailingList.Contacts.Add(contact);
            }
            db.Entry(mailingList).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await MailingListExists(id)) return false;
                else throw;
            }
            return true;
        }
        public async Task<bool> UpdateMailingList(int id, List<string> guids)
        {
            var mailingList = await db.MailingLists.FindAsync(id);
            Contact contact;
            foreach (var guid in guids)
            {
                contact = await db.Contacts.FirstOrDefaultAsync(x => x.Guid.ToString() == guid);
                if (mailingList.Contacts.Contains(contact)) mailingList.Contacts.Remove(contact);
                else mailingList.Contacts.Add(contact);
            }
            db.Entry(mailingList).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await MailingListExists(id)) return false;
                else throw;
            }
            return true;
        }
        public async Task<MailingListResponseModel> RemoveMailingList(int id)
        {
            var mailingList = await db.MailingLists.FindAsync(id);

            db.MailingLists.Remove(mailingList);
            db.SaveChanges();

            return modelFactory.CreateMailingListResponseModel(mailingList);
        }

        public async Task<bool> MailingListExists(int id)
        {
            return await db.MailingLists.CountAsync(e => e.ID == id) > 0;
        }
        #endregion
        #region Templates methods
        public async Task<List<TemplateResponseModel>> GetAllTemplates()
        {
            var templates = await db.Templates.ToListAsync();
            return templates.Select(modelFactory.CreateTemplateResponseModel).ToList();
        }

        public async Task<TemplateResponseModel> GetTemplateById(int id)
        {
            var template = await db.Templates.FirstOrDefaultAsync(x => x.Id == id);
            return template == null ? null : modelFactory.CreateTemplateResponseModel(template);
        }

        public async Task<bool> TemplateExists(int id)
        {
            return await db.Templates.CountAsync(e => e.Id == id) > 0;
        }

        #endregion
        public void Dispose()
        {
            db.Dispose();
        }
    }
}