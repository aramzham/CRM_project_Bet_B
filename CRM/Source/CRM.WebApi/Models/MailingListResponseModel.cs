using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRM.WebApi.Models
{
    public class MailingListResponseModel
    {
        public int MailingListId { get; set; }
        public string MailingListName { get; set; }
        public List<ContactResponseModel> Contacts { get; set; }
    }
}