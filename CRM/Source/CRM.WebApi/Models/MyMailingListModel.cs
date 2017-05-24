using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRM.EntityFramework;

namespace CRM.WebApi.Models
{
    public class MyMailingListModel
    {
        public MyMailingListModel()
        {

        }

        public MyMailingListModel(MailingList mlist)
        {
            MailingListName = mlist.MailingListName;
            Contacts = mlist.Contacts.Select(x => x.Email).ToList();
        }
        public int MailingListId { get; set; }
        public string MailingListName { get; set; }
        public List<string> Contacts { get; set; }
    }
}