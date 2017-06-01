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
        private ApplicationManager appManager = new ApplicationManager();

        // GET: api/MailingLists
        [Route("api/MailingLists")]
        public async Task<IHttpActionResult> GetMailingLists()
        {
            var mailingLists = await appManager.GetAllMailingLists();
            if (mailingLists == null) return BadRequest("Something went wrong, call your sysadmin");
            return Ok(mailingLists);
        }

        // GET: api/MailingLists/5
        [ResponseType(typeof(MailingList))]
        public async Task<IHttpActionResult> GetMailingList(int id)
        {
            var mailingList = await appManager.GetMailingListById(id);
            if (mailingList == null) return NotFound();
            return Ok(mailingList);
        }

        // PUT: api/MailingLists/add
        [ResponseType(typeof(void)), Route("api/MailingLists/add"), HttpPut]
        public async Task<HttpResponseMessage> AddContacts([FromUri]int[] ids, [FromBody]string[] guids)
        {
            if (!ModelState.IsValid) return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);

            var response = await appManager.AddContactsToMailingLists(ids, guids);
            if (response == null)
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError,
                    "Oops! Something went wrong, call your sysadmin");
            return response == "Success!" ? Request.CreateResponse(HttpStatusCode.NoContent, response) : Request.CreateErrorResponse(HttpStatusCode.Conflict, response);
        }

        //PUT: api/MailingLists/remove/{id}
        [ResponseType(typeof(void)), Route("api/MailingLists/remove"), HttpPut]
        public async Task<IHttpActionResult> PutMailingList(int id, [FromBody]string[] guids)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var response = await appManager.RemoveContactsFromMailingLists(id, guids);
            if (response == true) return Ok(StatusCode(HttpStatusCode.NoContent));
            else return BadRequest("Invalid input data");
        }

        // POST: api/MailingLists
        [ResponseType(typeof(MailingList)), HttpPost]
        public async Task<IHttpActionResult> CreateNewMailingList([FromUri]string mailingListName)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var mailingList = await appManager.AddMailingList(mailingListName);

            return CreatedAtRoute("DefaultApi", new { id = mailingList.MailingListId }, mailingList);
        }

        // POST: api/MailingLists
        [ResponseType(typeof(MailingList)), Route("api/MailingLists/new", Name = "CreateNewMailingList"), HttpPost]
        public async Task<IHttpActionResult> CreateMailingListFromContactsList([FromUri]string name, [FromBody]string[] guids)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var mailingList = await appManager.AddMailingList(name);
            var response = await appManager.AddContactsToMailingLists(new[] { mailingList.MailingListId }, guids);
            if (response == null) return BadRequest("Something went wrong, call your sysadmin");
            if (response != "Success!") return BadRequest(response);
            return CreatedAtRoute("CreateNewMailingList", new { id = mailingList.MailingListId }, mailingList);
        }

        // DELETE: api/MailingLists/5
        [ResponseType(typeof(MailingListResponseModel))]
        public async Task<IHttpActionResult> DeleteMailingList(int id)
        {
            if (!await appManager.MailingListExists(id)) return BadRequest();
            var mailingList = await appManager.RemoveMailingList(id);
            if (mailingList == null) return NotFound();
            return Ok(mailingList);
        }

        // DELETE: api/MailingLists
        [ResponseType(typeof(MailingListResponseModel)), Route("api/MailingLists")]
        public async Task<IHttpActionResult> DeleteSeveralMailingLists([FromBody]int[] ids)
        {
            var removedMailingLists = await appManager.RemoveSeveralMailingLists(ids);
            if (removedMailingLists == null) return BadRequest("One or more ids were not correct");
            return Ok(removedMailingLists);
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
