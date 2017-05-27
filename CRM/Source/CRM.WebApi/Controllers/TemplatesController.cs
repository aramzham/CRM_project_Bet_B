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
    public class TemplatesController : ApiController
    {
        private CRMDatabaseEntities db = new CRMDatabaseEntities();

        // GET: api/Templates
        public IQueryable<Template> GetTemplates()
        {
            return db.Templates;
        }

        // GET: api/Templates/5
        [ResponseType(typeof(Template))]
        public IHttpActionResult GetTemplate(int id)
        {
            Template template = db.Templates.Find(id);
            if (template == null)
            {
                return NotFound();
            }

            return Ok(template);
        }

        // PUT: api/Templates/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutTemplate(int id, Template template)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != template.Id)
            {
                return BadRequest();
            }

            db.Entry(template).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TemplateExists(id))
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

        // POST: api/Templates
        [ResponseType(typeof(Template))]
        public IHttpActionResult PostTemplate(Template template)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Templates.Add(template);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = template.Id }, template);
        }

        // DELETE: api/Templates/5
        [ResponseType(typeof(Template))]
        public IHttpActionResult DeleteTemplate(int id)
        {
            Template template = db.Templates.Find(id);
            if (template == null)
            {
                return NotFound();
            }

            db.Templates.Remove(template);
            db.SaveChanges();

            return Ok(template);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TemplateExists(int id)
        {
            return db.Templates.Count(e => e.Id == id) > 0;
        }
    }
}