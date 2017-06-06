using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace CRM.WebApi.Models
{
    public class ContactRequestModel
    {
        //RegularExpression("/^[a-zA-ZàáâäãåąčćęèéêëėįìíîïłńòóôöõøùúûüųūÿýżźñçčšžÀÁÂÄÃÅĄĆČĖĘÈÉÊËÌÍÎÏĮŁŃÒÓÔÖÕØÙÚÛÜŲŪŸÝŻŹÑßÇŒÆČŠŽ∂ð ,.'-]+$/u") for international names
        [Required(ErrorMessage = "Full name is required"), StringLength(200, MinimumLength = 1, ErrorMessage = "The full name must be specified.")]
        public string FullName { get; set; }
        [Required(ErrorMessage = "Company name is required"), StringLength(150, MinimumLength = 1, ErrorMessage = "The company name must be specified.")]
        public string CompanyName { get; set; }
        [Required(ErrorMessage = "Position is required"), StringLength(100, MinimumLength = 1, ErrorMessage = "The position must be specified.")]
        public string Position { get; set; }
        [Required(ErrorMessage = "Country is required"), StringLength(50, MinimumLength = 1, ErrorMessage = "The full name must be specified.")]
        public string Country { get; set; }
        [Required(ErrorMessage = "Email address is required"), EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
    }
}