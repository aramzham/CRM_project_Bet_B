using System.ComponentModel.DataAnnotations;

namespace CRM.WebApi.Models
{
    public class ContactRequestModel
    {
        [Required(ErrorMessage = "Full name is required")]
        public string FullName { get; set; }
        public string CompanyName { get; set; }
        public string Position { get; set; }
        public string Country { get; set; }
        [Required(ErrorMessage = "Email address is required")]
        public string Email { get; set; }
    }
}