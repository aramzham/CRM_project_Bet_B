using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRM.WebApi.Models
{
    public class QueryRequestModel
    {
        public Dictionary<string, string> FullName { get; set; }
        public Dictionary<string, string> CompanyName { get; set; }
        public Dictionary<string, string> Position { get; set; }
        public Dictionary<string, string> Country { get; set; }
        public Dictionary<string, string> Email { get; set; }
    }
}