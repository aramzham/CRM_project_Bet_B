using Microsoft.AspNet.Identity.EntityFramework;

namespace CRM.WebApi.Models.AuthModels
{
    public class AuthContext : IdentityDbContext<IdentityUser>
    {
        public AuthContext()
            : base("AuthContext")
        {

        }
    }
}