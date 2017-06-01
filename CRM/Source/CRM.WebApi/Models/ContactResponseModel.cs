using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace CRM.WebApi.Models
{
    public class ContactResponseModel
    {
        [JsonProperty("Full name")]
        public string FullName { get; set; }
        [JsonProperty("Company name")]
        public string CompanyName { get; set; }
        public string Position { get; set; }
        public string Country { get; set; }
        public string Email { get; set; }
        public Guid? Guid { get; set; }
        [JsonProperty("Mailing lists")]
        public List<string> MailingLists { get; set; }
    }
}