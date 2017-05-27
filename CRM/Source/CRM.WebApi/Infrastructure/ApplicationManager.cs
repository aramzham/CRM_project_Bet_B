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

        public async Task<List<ContactResponseModel>> GetAllContacts()
        {
            return await db.Contacts.Select(x => ModelFactory.CreateContactResponseModel(x)).ToListAsync();
        }

        public async Task<Contact> GetContactById(int id)
        {
            return await db.Contacts.FindAsync(id);
        }

        public async Task<ContactResponseModel> GetContactByGuid([FromUri] string guid)
        {
            return ModelFactory.CreateContactResponseModel(await db.Contacts.Where(x => x.Guid.ToString() == guid).FirstOrDefaultAsync());
        }

        public async Task<List<Contact>> GetByPage(int start, int numberOfRows, bool @ascending)
        {
            if (ascending) return await db.Contacts.OrderBy(x => x.ID).Skip(start - 1).Take(numberOfRows).ToListAsync();
            else return await db.Contacts.OrderByDescending(x => x.ID).Skip(start - 1).Take(numberOfRows).ToListAsync();
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

        public async Task<bool> UpdateContact(string guid, ContactRequestModel contact)
        {
            var contactToUpdate = await GetContactByGuid(guid);
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
                if (!await ContactExists(guid)) return false;
                else throw;
            }
            return true;
        }
        public async Task<Contact> AddContact(Contact contact)
        {
            contact.Guid = Guid.NewGuid();
            contact.DateInserted = DateTime.Now;

            db.Contacts.Add(contact);
            await db.SaveChangesAsync();

            return contact;
        }
        public async Task<Contact> AddContact(ContactRequestModel requestContact)
        {
            var contactToAdd = ModelFactory.CreateContact(requestContact);
            db.Contacts.Add(contactToAdd);
            await db.SaveChangesAsync();

            return contactToAdd;
        }

        public async Task<Contact> RemoveContact(int id)
        {
            var contact = await GetContactById(id);

            db.Contacts.Remove(contact);
            await db.SaveChangesAsync();

            return contact;
        }

        public async Task<Contact> RemoveContact(string guid)
        {
            var contact = await db.Contacts.Where(x => x.Guid.ToString() == guid).FirstOrDefaultAsync();

            db.Contacts.Remove(contact);
            await db.SaveChangesAsync();

            return contact;
        }

        public async Task<bool> ContactExists(int id)
        {
            return await db.Contacts.CountAsync(e => e.ID == id) > 0;
        }

        public async Task<bool> ContactExists(string guid)
        {
            return await db.Contacts.CountAsync(e => e.Guid.ToString() == guid) > 0;
        }

        public void Dispose()
        {
            db.Dispose();
        }
    }
}