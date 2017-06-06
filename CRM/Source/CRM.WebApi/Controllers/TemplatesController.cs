using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using CRM.WebApi.Infrastructure;

namespace CRM.WebApi.Controllers
{
    [NotImplExceptionFilter]
    public class TemplatesController : ApiController
    {
        private LoggerManager logger = new LoggerManager();
        private ApplicationManager appManager = new ApplicationManager();

        // GET: api/Templates
        public async Task<IHttpActionResult> GetTemplates()
        {
            var templates = await appManager.GetAllTemplates();
            if (templates == null) return BadRequest("Something went wrong, call your supporting stuff");
            else return Ok(templates);
        }

        //GET: api/Templates/Exceptions
        [Route("api/Templates/Exceptions"), HttpGet]
        public HttpResponseMessage GetLog()
        {
            var response = new HttpResponseMessage { Content = new StringContent(logger.ReadLogData()) };
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return response;
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