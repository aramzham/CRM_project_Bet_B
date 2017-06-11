using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using CRM.WebApi.Infrastructure;
using CRM.WebApi.Infrastructure.ApplicationManagers;
using CRM.WebApi.Models;

namespace CRM.WebApi.Controllers
{
    [NotImplExceptionFilter]
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

        [Authorize]
        // GET: api/Contacts?Guid=guid
        [ResponseType(typeof(ContactResponseModel))]
        public async Task<IHttpActionResult> GetContactByGuid([FromUri]string guid)
        {
            var contact = await appManager.GetContactByGuid(guid);
            if (contact == null) return NotFound();

            return Ok(contact);
        }

        //GET: api/Contacts/demo
        [ResponseType(typeof(void)), Route("api/Contacts/reset"), HttpGet]
        public async Task<HttpResponseMessage> GetDemo()
        {
            var content = await appManager.ResetForDemo();
            if (content == null) return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Oops! Something went wrong");
            var response = new HttpResponseMessage { Content = new StringContent(content) };
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return response;
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

            if (!await appManager.UpdateContact(guid, contact)) return BadRequest("Contact not found or specified email already exists");
            else return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Contacts
        [ResponseType(typeof(ContactRequestModel))]
        public async Task<IHttpActionResult> PostContact([FromBody]ContactRequestModel contact)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if ((await appManager.GetAllEmails()).Contains(contact.Email))
                return BadRequest("A contact with such email already exists");

            var responseContact = await appManager.AddContact(contact);

            return CreatedAtRoute("DefaultApi", new { }, responseContact); //shows up in location header
        }

        //POST: api/Contacts/upload
        [Route("api/Contacts/upload"), HttpPost]
        public async Task<IHttpActionResult> PostFormData()
        {
            if (!Request.Content.IsMimeMultipartContent()) throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

            var root = HttpContext.Current.Server.MapPath("~//Templates");
            var provider = new MultipartFormDataStreamProvider(root);
            await Request.Content.ReadAsMultipartAsync(provider);

            var parser = new ParsingManager();
            var buffer = File.ReadAllBytes(provider.FileData.SingleOrDefault()?.LocalFileName);
            var contacts = parser.RetrieveContactsFromFile(buffer);
            if (contacts == null) return BadRequest(message: "Invalid file");
            var success = await appManager.AddMultipleContacts(contacts);

            if (success == false || contacts.TrueForAll(x => x == null)) return BadRequest(message: "File or data is corrupt");
            return Ok($"Added: {contacts.Count(x => x != null)} contact(s), Failed: {contacts.Count(x => x == null)}");
        }

        //POST: api/Contacts/query
        [ResponseType(typeof(ContactResponseModel)), Route("api/Contacts/query"), HttpPost]
        public async Task<IHttpActionResult> Query([FromUri]string[] sort, [FromBody] QueryRequestModel filter)
        {
            var queryResult = await appManager.Query(sort, filter);
            if (queryResult == null) return BadRequest("Invalid query");
            if (queryResult.Count == 0) return Ok("Your query didn't produce anything");
            return Ok(queryResult);
        }

        // DELETE: api/Contacts/guid
        [ResponseType(typeof(ContactResponseModel))]
        public async Task<IHttpActionResult> DeleteContact(string guid)
        {
            var contact = await appManager.RemoveContact(guid);
            if (contact == null) return NotFound();
            return Ok(contact);
        }

        // DELETE: api/Contacts
        [ResponseType(typeof(ContactResponseModel))]
        public async Task<IHttpActionResult> DeleteContactByGroup([FromBody]string[] guids)
        {
            var contacts = await appManager.RemoveContactByGroup(guids);
            if (contacts == null) return BadRequest("One or more guids were corrupt");
            return Ok(contacts);
        }
    }
}
