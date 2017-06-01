using CRM.EntityFramework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using CRM.WebApi.Infrastructure;
using CRM.WebApi.Models;

namespace CRM.WebApi.Controllers
{
    //TODO: rebuild the parser
    //TODO: IHttpActionResult poxel httpResponseMessage
    //TODO: parseri mej stugel fullname, email null chlni, email-er@ valid linen
    public class ContactsController : ApiController
    {
        private ApplicationManager appManager = new ApplicationManager();

        // GET: api/Contacts
        public async Task<IHttpActionResult> GetContacts()
        {
            var contacts = await appManager.GetAllContacts();
            if (contacts == null) return BadRequest();
            return Ok(contacts);
        }

        // GET: api/Contacts?Guid=guid
        [ResponseType(typeof(ContactResponseModel))]
        public async Task<IHttpActionResult> GetContactByGuid([FromUri]string guid)
        {
            var contact = await appManager.GetContactByGuid(guid);
            if (contact == null) return NotFound();

            return Ok(contact);
        }

        //// GET: api/Contacts/?start=1&numberOfRows=2&ascending=false
        //[ResponseType(typeof(Contact))]
        //public async Task<IHttpActionResult> GetContact(int start, int numberOfRows, bool ascending)
        //{
        //    //start should be 1-based (f.e. if you want from first record, then type 1)
        //    var contacts = await appManager.GetByPage(start, numberOfRows, ascending);

        //    if (contacts == null) return NotFound();

        //    return Ok(contacts);
        //}

        [ResponseType(typeof(void))] // PUT: api/Contacts?Guid=guid
        public async Task<IHttpActionResult> PutContact(string guid, [FromBody]ContactRequestModel contact)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            //if (contact.Guid == null || guid != contact.Guid.ToString()) return BadRequest();

            if (!await appManager.UpdateContact(guid, contact)) return BadRequest("Contact not found or specified email already exists");
            else return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Contacts
        [ResponseType(typeof(ContactRequestModel))]
        public async Task<IHttpActionResult> PostContact([FromBody]ContactRequestModel contact)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (!Regex.IsMatch(contact.Email, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase)) return BadRequest("Email address is not valid");

            if ((await appManager.GetAllEmails()).Contains(contact.Email))
                return BadRequest("A contact with such email already exists");

            var responseContact = await appManager.AddContact(contact);

            return CreatedAtRoute("DefaultApi", new { }, responseContact); //shows up in location header
        }

        //// POST: api/Contacts/upload
        //[ResponseType(typeof(ContactRequestModel)), Route("api/Contacts/upload"), HttpPost]
        //public async Task<IHttpActionResult> Upload([FromBody]byte[] fileBytes)
        //{
        //    try
        //    {
        //        var parser = new ParsingManager();
        //        var contacts = parser.RetrieveContactsFromFile(fileBytes);
        //        foreach (var contact in contacts)
        //        {
        //            await appManager.AddContact(contact);
        //        }
        //        return Ok();
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        [Route("api/Contacts/upload"), HttpPost]
        public async Task<HttpResponseMessage> PostFormData()
        {
            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            string root = HttpContext.Current.Server.MapPath("~/App_Data");
            var provider = new MultipartFormDataStreamProvider(root);

            try
            {
                // Read the form data.
                await Request.Content.ReadAsMultipartAsync(provider);

                // This illustrates how to get the file names.
                var fileNames = new List<string>();
                var contacts = new List<Contact>();
                foreach (var file in provider.FileData)
                {
                    //fileNames.Add(file.Headers.ContentDisposition.FileName.Trim('\"'));
                    var buffer = File.ReadAllBytes(file.LocalFileName);
                    var parser = new ParsingManager();
                    contacts = parser.RetrieveContactsFromFile(buffer);
                    //hash values, reject request if needed
                }
                return Request.CreateResponse(HttpStatusCode.OK, contacts);
            }
            catch (System.Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        //[Route("api/Contacts/upload"), HttpPost]
        //public HttpResponseMessage Post()
        //{
        //    HttpResponseMessage result = null;
        //    var httpRequest = HttpContext.Current.Request;
        //    if (httpRequest.Files.Count > 0)
        //    {
        //        var docfiles = new List<string>();
        //        foreach (string file in httpRequest.Files)
        //        {
        //            var postedFile = httpRequest.Files[file];
        //            var filePath = HttpContext.Current.Server.MapPath("~/App_Data/" + postedFile.FileName);
        //            postedFile.SaveAs(filePath);

        //            docfiles.Add(filePath);
        //        }
        //        result = Request.CreateResponse(HttpStatusCode.Created, docfiles);
        //    }
        //    else
        //    {
        //        result = Request.CreateResponse(HttpStatusCode.BadRequest);
        //    }
        //    return result;
        //}

        // DELETE: api/Contacts/guid
        [ResponseType(typeof(ContactResponseModel))]
        public async Task<IHttpActionResult> DeleteContact(string guid)
        {
            var contact = await appManager.RemoveContact(guid);
            if (contact == null) return NotFound();
            else return Ok(contact);
        }

        // DELETE: api/Contacts
        [ResponseType(typeof(ContactResponseModel))]
        public async Task<IHttpActionResult> DeleteContactByGroup([FromBody]string[] guids)
        {
            var contacts = await appManager.RemoveContactByGroup(guids);
            if (contacts == null || contacts.Any(x => x == null)) return BadRequest("One or more guids were corrupt");
            else return Ok(contacts);
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
