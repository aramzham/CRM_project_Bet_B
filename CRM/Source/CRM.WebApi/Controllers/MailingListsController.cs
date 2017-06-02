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
using System.Web.Http.Results;
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
        [ResponseType(typeof(void)), Route("api/MailingLists/add/{id}"), HttpPut]
        public async Task<HttpResponseMessage> AddContactsIntoMailingList(int id, [FromBody]string[] guids)
        {
            if (!ModelState.IsValid) return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);

            var mailingListResponseModel = await appManager.AddContactsToMailingLists(id, guids);
            return mailingListResponseModel == null ? Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Input data was invalid") : Request.CreateResponse(HttpStatusCode.NoContent, mailingListResponseModel);
        }

        //PUT: api/MailingLists/remove/{id}
        [ResponseType(typeof(MailingListResponseModel)), Route("api/MailingLists/remove/{id}"), HttpPut]
        public async Task<IHttpActionResult> RemoveContactsFromMailingList(int id, [FromBody]string[] guids)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var mailingListResponseModel = await appManager.RemoveContactsFromMailingList(id, guids);
            if (mailingListResponseModel != null) return Ok(mailingListResponseModel);
            else return BadRequest("Invalid input data");
        }

        // PUT: api/MailingLists/rename/id
        [ResponseType(typeof (MailingListResponseModel)), Route("api/MailingLists/rename/{id}"), HttpPut]
        public async Task<IHttpActionResult> RenameMailingList(int id, [FromBody] string name)
        {
            if (string.IsNullOrEmpty(name)) return BadRequest("Please specify a valid name");
            var mailingList = await appManager.RenameMailingList(id, name);
            if (mailingList == null) return BadRequest("Something went wrong, invalid input data");
            else return Ok(mailingList);
        }

        // POST: api/MailingLists
        [ResponseType(typeof(MailingListResponseModel)),Route("api/MailingLists", Name = "CreateEmptyMailingList"), HttpPost]
        public async Task<IHttpActionResult> CreateNewMailingList([FromUri]string mailingListName)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var mailingList = await appManager.AddMailingList(mailingListName);

            return CreatedAtRoute("CreateEmptyMailingList", new { id = mailingList.MailingListId }, mailingList);
        }

        // POST: api/MailingLists
        [ResponseType(typeof(MailingListResponseModel)), Route("api/MailingLists/new", Name = "CreateNewMailingList"), HttpPost]
        public async Task<IHttpActionResult> CreateMailingListFromContactsList([FromUri]string name, [FromBody]string[] guids)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var mailingList = await appManager.AddMailingList(name);
            var mailingListResponseModel = await appManager.AddContactsToMailingLists(mailingList.MailingListId, guids);
            if (mailingListResponseModel == null) return BadRequest("Error: Invalid input data");
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
