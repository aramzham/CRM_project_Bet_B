using CRM.EntityFramework;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Formatting;
using Newtonsoft.Json;

namespace ConsoleClient
{
    class Program
    {
        private static string currentAddress = @"http://localhost:56416/";
        static void Main(string[] args)
        {
            var cont1 = new Contact { FullName = "Arsen Grigoryan", CompanyName = "Mshakuyti naxararutyun", Position = "Texakal", Country = "Armenia", Email = "agrig@freenet.am" };
            var cont2 = new Contact { FullName = "Felix Derjinskiy", CompanyName = "Whites", Position = "Head", Country = "Russia", Email = "felo@rambler.ru" };
            var cont3 = new Contact { FullName = "Fox2000", CompanyName = "FoxKids", Position = "Picture", Country = "USA", Email = "fk@us.com" };

            var client = new HttpClient();
            var response = client.PostAsync($"{currentAddress}api/Contacts", cont3, new JsonMediaTypeFormatter()).Result;
            Console.WriteLine(response.IsSuccessStatusCode
                ? "Record/Records added successfully!"
                : response.Content.ToString());

            var result = client.GetAsync($"{currentAddress}api/Contacts").Result;
            var json = result.Content.ReadAsStringAsync().Result;
            var data = (List<Contact>)JsonConvert.DeserializeObject(json, typeof(List<Contact>));
            foreach (var contact in data)
            {
                Console.Write($"{contact.ContactId}. {contact.FullName} {contact.CompanyName} {contact.Position} {contact.Country} {contact.Email} {contact.Guid} {contact.DateInserted}\n");
            }

            Console.ReadKey();
        }
    }
}
