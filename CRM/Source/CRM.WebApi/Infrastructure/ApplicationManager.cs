using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using CRM.EntityFramework;

namespace CRM.WebApi.Infrastructure
{
    public class ApplicationManager : IDisposable
    {
        private CRMDatabaseEntities db = new CRMDatabaseEntities();

        public async Task<List<Contact>> GetAllContacts()
        {
            return await db.Contacts.ToListAsync();
        }

        public async Task<Contact> GetContactByID(int id)
        {
            return await db.Contacts.FindAsync(id);
        }

        public async Task<Contact> GetContactByGuid([FromUri] string guid)
        {
            return await db.Contacts.Where(x => x.Guid.ToString() == guid).FirstOrDefaultAsync();
        }

        public async Task<List<Contact>> GetByPage(int start, int numberOfRows, bool @ascending)
        {
            if (ascending) return await db.Contacts.OrderBy(x => x.ID).Skip(start - 1).Take(numberOfRows).ToListAsync();
            else return await db.Contacts.OrderByDescending(x => x.ID).Skip(start - 1).Take(numberOfRows).ToListAsync();
        }

        public async void UpdateContact(int id, Contact contact)
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
        }

        public void AddContact(Contact contact)
        {
            contact.Guid = Guid.NewGuid();
            contact.DateInserted = DateTime.UtcNow;

            db.Contacts.Add(contact);
        }

        public void RemoveContact(Contact contact)
        {
            db.Contacts.Remove(contact);
        }

        public async void SaveDb()
        {
            await db.SaveChangesAsync();
        }

        public bool ContactExists(int id)
        {
            return db.Contacts.Count(e => e.ID == id) > 0;
        }

        public void Dispose()
        {
            db.Dispose();
        }
    }
}