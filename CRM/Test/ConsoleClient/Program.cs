using CRM.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ConsoleClient
{
    class Program
    {
        private static string currentAddress = @"http://localhost:58017/";
        static void Main(string[] args)
        {
            var st1 = new Contact { FullName = "a1 a1yan", CompanyName = "c1", Position = "p1", Country = "country1", Email = "a1@c1.com" };
            var st2 = new Contact { FullName = "a2 a2yan", CompanyName = "c2", Position = "p2", Country = "country2", Email = "a2@c2.com" };
            var st3 = new Contact { FullName = "a3 a3yan", CompanyName = "c3", Position = "p3", Country = "country3", Email = "a3@c3.com" };

            var client = new HttpClient();
            var response = client.PostAsync($"{currentAddress}api/Contacts", st1, new JsonMediaTypeFormatter()).Result;
            Console.WriteLine(response.IsSuccessStatusCode
                ? "Record/Records added successfully!"
                : response.Content.ToString());

            var result = client.GetAsync($"{currentAddress}api/Contacts").Result;
            var json = response.Content.ReadAsStringAsync().Result;
            var data = (List<Contact>)JsonConvert.DeserializeObject(json, typeof(List<Contact>));
            foreach (var contact in data)
            {
                Console.Write($"{contact.ContactId}. {contact.FullName} {contact.CompanyName} {contact.Position} {contact.Country} {contact.Email}\n");
            }
        }
    }
}
