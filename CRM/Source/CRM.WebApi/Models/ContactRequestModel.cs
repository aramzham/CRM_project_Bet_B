using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace CRM.WebApi.Models
{
    public class ContactRequestModel
    {
        //RegularExpression("/^[a-zA-ZàáâäãåąčćęèéêëėįìíîïłńòóôöõøùúûüųūÿýżźñçčšžÀÁÂÄÃÅĄĆČĖĘÈÉÊËÌÍÎÏĮŁŃÒÓÔÖÕØÙÚÛÜŲŪŸÝŻŹÑßÇŒÆČŠŽ∂ð ,.'-]+$/u") for international names
        [Required(ErrorMessage = "Full name is required"), RegularExpression("/^[a-z ,.'-]+$/i", ErrorMessage = "We don't work with such clients")]
        public string FullName { get; set; }
        public string CompanyName { get; set; }
        public string Position { get; set; }
        public string Country { get; set; }
        [Required(ErrorMessage = "Email address is required"), EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
    }
}