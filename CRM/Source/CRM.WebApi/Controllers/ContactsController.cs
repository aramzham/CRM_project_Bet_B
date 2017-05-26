using CRM.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using CRM.WebApi.Infrastructure;

namespace CRM.WebApi.Controllers
{
    public class ContactsController : ApiController
    {
        private ApplicationManager appManager = new ApplicationManager();

        // GET: api/Contacts
        public async Task<List<Contact>> GetContacts()
        {
            return await appManager.GetAllContacts();
        }

        // GET: api/Contacts/5
        [ResponseType(typeof(Contact))]
        public async Task<IHttpActionResult> GetContact(int id)
        {
            var contact = await appManager.GetContactByID(id);
            if (contact == null)
            {
                return NotFound();
            }

            return Ok(contact);
        }

        [ResponseType(typeof (Contact))]
        public async Task<IHttpActionResult> GetContactByGuid([FromUri]string guid)
        {
            var contact = await appManager.GetContactByGuid(guid);
            if (contact == null)
            {
                return NotFound();
            }

            return Ok(contact);
        }

        // GET: api/Contacts/?start=1&numberOfRows=2&ascending=false
        [ResponseType(typeof(Contact))]
        public async Task<IHttpActionResult> GetContact(int start, int numberOfRows, bool ascending)
        {
            //start should be 1-based (f.e. if you want from first record, then type 1)
            var contacts = await appManager.GetByPage(start, numberOfRows, ascending);

            if (contacts == null)
            {
                return NotFound();
            }

            return Ok(contacts);
        }

        // PUT: api/Contacts/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutContact(int id, [FromBody]Contact contact)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (id != contact.ID) return BadRequest();

            appManager.UpdateContact(id, contact);

            try
            {
                appManager.SaveDb();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!appManager.ContactExists(id)) return NotFound();
                else throw;
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Contacts
        [ResponseType(typeof(Contact))]
        public async Task<IHttpActionResult> PostContact([FromBody]Contact contact)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            appManager.AddContact(contact);
            
            appManager.SaveDb();

            return CreatedAtRoute("DefaultApi", new { id = contact.ID }, contact);
        }

        // DELETE: api/Contacts/5
        [ResponseType(typeof(Contact))]
        public async Task<IHttpActionResult> DeleteContact(int id)
        {
            var contact = await appManager.GetContactByID(id);
            if (contact == null) return NotFound();

            appManager.RemoveContact(contact);
            appManager.SaveDb();

            return Ok(contact);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                appManager.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
