using CRM.EntityFramework;
using LinqToExcel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace CRM.WebApi.Infrastructure
{
    public class ParsingManager
    {
        private static readonly Dictionary<Extensions, string> ExtensionSignature = new Dictionary<Extensions, string>
        {
            {Extensions.CSV, "66-69-6C-65-2C-66-6F-72"},
            {Extensions.Xlsx, "50-4B-03-04-14-00-06-00"}
        };

        public List<Contact> RetrieveContactsFromFile(byte[] bytes)
        {
            var contacts = new List<Contact>();
            Extensions currentExtension = GetExtension(bytes);
            string path = "file";

            try
            {
                File.WriteAllBytes(path, bytes);

                contacts = currentExtension == Extensions.Xlsx ? RetrieveContactsFromExcel(path) : RetrieveContactsFromCsv(path);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                File.Delete(path);
            }
            return contacts;
        }

        private static List<Contact> RetrieveContactsFromExcel(string path)
        {
            var excel = new ExcelQueryFactory(path);
            var sheets = excel.GetWorksheetNames();
            var contactsRows = (from c in excel.Worksheet<Row>(sheets.First())
                                select c).ToList();

            return contactsRows.Select(contactRow => new Contact
            {
                FullName = contactRow["FullName"], CompanyName = contactRow["CompanyName"], Position = contactRow["Position"], Country = contactRow["Country"], Email = contactRow["Email"]
            }).ToList();
        }

        private static List<Contact> RetrieveContactsFromCsv(string path)
        {
            var contacts = new List<Contact>();
            var lines = File.ReadAllLines(path);

            for (int i = 1; i < lines.Length; i++)
            {
                var currentLine = lines[i].Split(',');
                var contact = new Contact
                {
                    FullName = currentLine[0],
                    CompanyName = currentLine[1],
                    Position = currentLine[2],
                    Country = currentLine[3],
                    Email = currentLine[4]
                };
                //contact.Guid = Guid.NewGuid();
                //contact.DateInserted = 
                contacts.Add(contact);
            }
            return contacts;
        }

        private enum Extensions
        {
            CSV,
            Xlsx,
        }


        private static Extensions GetExtension(byte[] bytes)
        {
            if (bytes.Length < 8)
                throw new ArgumentOutOfRangeException();
            var signatureBytes = new byte[8];
            Array.Copy(bytes, signatureBytes, signatureBytes.Length);
            string signature = BitConverter.ToString(signatureBytes);
            Extensions extension = ExtensionSignature.FirstOrDefault(pair => signature.Contains(pair.Value)).Key;

            switch (extension)
            {
                case Extensions.CSV:
                    return extension;
                case Extensions.Xlsx:
                    string fileBody = Encoding.UTF8.GetString(bytes);
                    if (fileBody.Contains("xl"))
                        return extension;
                    break;
            }
            throw new ArgumentOutOfRangeException("The format of uploaded file was incorrect. Only .xlsx and .csv are allowed.");
        }
    }
}