using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using CRM.EntityFramework;

namespace CRM.WebApi.Controllers
{
    public class ContactsController : ApiController
    {
        private CRMDatabaseEntities db = new CRMDatabaseEntities();

        // GET: api/Contacts
        public List<Contact> GetContacts()
        {
            return db.Contacts.ToList();
        }

        // GET: api/Contacts/5
        [ResponseType(typeof(Contact))]
        public IHttpActionResult GetContact(int id)
        {
            var contact = db.Contacts.FindAsync(id).Result;
            if (contact == null)
            {
                return NotFound();
            }

            return Ok(contact);
        }

        // PUT: api/Contacts/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutContact(int id, [FromBody]Contact contact)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (id != contact.ContactId) return BadRequest();

            db.Entry(contact).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContactExists(id)) return NotFound();
                else throw;
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Contacts
        [ResponseType(typeof(Contact))]
        public IHttpActionResult PostContact([FromBody]Contact contact)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            contact.Guid = Guid.NewGuid();
            contact.DateInserted = DateTime.UtcNow;

            db.Contacts.Add(contact);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = contact.ContactId }, contact);
        }

        // DELETE: api/Contacts/5
        [ResponseType(typeof(Contact))]
        public IHttpActionResult DeleteContact(int id)
        {
            var contact = db.Contacts.FindAsync(id).Result;
            if (contact == null) return NotFound();

            db.Contacts.Remove(contact);
            db.SaveChanges();

            return Ok(contact);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ContactExists(int id)
        {
            return db.Contacts.Count(e => e.ContactId == id) > 0;
        }
    }
}