using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CRM.EntityFramework;

namespace CRM.WebApi.Controllers
{
    public class ContactsController : ApiController
    {
        private CRMDatabaseEntities db = new CRMDatabaseEntities();

        // GET: api/Contacts
        public IEnumerable<Contact> Get()
        {
            return db.Contacts.ToList();
        }

        // GET: api/Contacts/5
        public Contact Get(int id)
        {
            return db.Contacts.Find(id);
        }

        // POST: api/Contacts
        public async void Post([FromBody]Contact contact)
        {
            db.Contacts.Add(contact);
            await db.SaveChangesAsync();
        }

        // PUT: api/Contacts/5
        public async void Put(int id, [FromBody]Contact value)
        {
            var contactToUpdate = await db.Contacts.FindAsync(id);
            contactToUpdate = value;
            db.Entry(contactToUpdate).State = EntityState.Modified;
            await db.SaveChangesAsync();
        }

        // DELETE: api/Contacts/5
        public void Delete(int id)
        {
            var contactToDelete = db.Contacts.FirstOrDefault(x => x.ContactId == id);
            if (contactToDelete != null) db.Contacts.Remove(contactToDelete);
        }
    }
}
