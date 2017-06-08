using System.Threading.Tasks;
using System.Web.Http;
using CRM.WebApi.Infrastructure;
using CRM.WebApi.Models.AuthModels;
using Microsoft.AspNet.Identity;

namespace CRM.WebApi.Controllers
{
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        private AuthUserManager _aum = null;

        public AccountController()
        {
            _aum = new AuthUserManager();
        }

        // POST api/Account/Register
        [AllowAnonymous]
        [Route("Register")]
        public async Task<IHttpActionResult> Register(UserModel userModel)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            IdentityResult result = await _aum.RegisterUser(userModel);

            IHttpActionResult errorResult = GetErrorResult(result);

            return errorResult ?? Ok();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _aum.Dispose();
            }

            base.Dispose(disposing);
        }

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null) return InternalServerError();

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                // No ModelState errors are available to send, so just return an empty BadRequest.
                if (ModelState.IsValid) return BadRequest();

                return BadRequest(ModelState);
            }
            return null;
        }
    }
}
