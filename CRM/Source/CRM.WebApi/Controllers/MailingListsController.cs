using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using CRM.EntityFramework;
using CRM.WebApi.Models;

namespace CRM.WebApi.Controllers
{
    public class MailingListsController : ApiController
    {
        private CRMDatabaseEntities db = new CRMDatabaseEntities();

        // GET: api/MailingLists
        public List<MyMailingListModel> GetMailingLists()
        {
            var dbMailingLists = db.MailingLists.ToList();

            return dbMailingLists.Select(dbMailingList => new MyMailingListModel(dbMailingList)).ToList();
        }

        // GET: api/MailingLists/5
        [ResponseType(typeof(MailingList))]
        public IHttpActionResult GetMailingList(int id)
        {
            var mailingList = db.MailingLists.Find(id);
            if (mailingList == null)
            {
                return NotFound();
            }

            return Ok(mailingList);
        }

        // PUT: api/MailingLists/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutMailingList(int id, MailingList mailingList)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != mailingList.ID)
            {
                return BadRequest();
            }

            db.Entry(mailingList).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MailingListExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/MailingLists
        [ResponseType(typeof(MailingList))]
        public IHttpActionResult PostMailingList(MailingList mailingList)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.MailingLists.Add(mailingList);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = mailingList.ID }, mailingList);
        }

        // DELETE: api/MailingLists/5
        [ResponseType(typeof(MailingList))]
        public IHttpActionResult DeleteMailingList(int id)
        {
            MailingList mailingList = db.MailingLists.Find(id);
            if (mailingList == null)
            {
                return NotFound();
            }

            db.MailingLists.Remove(mailingList);
            db.SaveChanges();

            return Ok(mailingList);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool MailingListExists(int id)
        {
            return db.MailingLists.Count(e => e.ID == id) > 0;
        }
    }
}
