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
using CRM.EntityFramework;
using CRM.WebApi.Infrastructure;
using CRM.WebApi.Models;

namespace CRM.WebApi.Controllers
{
    public class MailingListsController : ApiController
    {
        //private CRMDatabaseEntities db = new CRMDatabaseEntities();
        private ApplicationManager appManager = new ApplicationManager();

        // GET: api/MailingLists
        public async Task<List<MailingListResponseModel>> GetMailingLists()
        {
            return await appManager.GetAllMailingLists();
        }

        // GET: api/MailingLists/5
        [ResponseType(typeof(MailingList))]
        public async Task<IHttpActionResult> GetMailingList(int id)
        {
            var mailingList = await appManager.GetMailingListById(id);
            if (mailingList == null) return NotFound();
            return Ok(mailingList);
        }

        //// PUT: api/MailingLists/5
        //[ResponseType(typeof(void))]
        //public async Task<IHttpActionResult> PutMailingList(int id, [FromBody]List<ContactRequestModel> contacts)
        //{
        //    if (!ModelState.IsValid) return BadRequest(ModelState);

        //    if (!await appManager.MailingListExists(id)) return BadRequest();

        //    if (await appManager.UpdateMailingList(id, contacts)) return StatusCode(HttpStatusCode.NoContent);
        //    else return NotFound();
        //}

        // PUT: api/MailingLists/5
        [ResponseType(typeof(void))]
        [Route("api/MailingLists/add/{id}")]
        public async Task<IHttpActionResult> PutMailingList(int id, [FromBody]List<string> guids)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (!await appManager.MailingListExists(id)) return BadRequest();

            if (await appManager.UpdateMailingList(id, guids)) return StatusCode(HttpStatusCode.NoContent);
            else return NotFound();
        }

        // POST: api/MailingLists
        [ResponseType(typeof(MailingList))]
        public async Task<IHttpActionResult> PostMailingList([FromBody]MailingListRequestModel mailingListRequest)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var mailingList = await appManager.AddMailingList(mailingListRequest);

            return CreatedAtRoute("DefaultApi", new { id = mailingList.ID }, mailingList);
        }

        // DELETE: api/MailingLists/5
        [ResponseType(typeof(MailingList))]
        public async Task<IHttpActionResult> DeleteMailingList(int id)
        {
            if (!await appManager.MailingListExists(id)) return BadRequest();
            var mailingList = await appManager.RemoveMailingList(id);
            if (mailingList == null) return NotFound();
            return Ok(mailingList);
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
