using CRM.EntityFramework;
using LinqToExcel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using CRM.WebApi.Models;

namespace CRM.WebApi.Infrastructure
{
    public class ParsingManager
    {
        private ModelFactory modelFactory = new ModelFactory();

        private static readonly Dictionary<Extensions, string> ExtensionSignature = new Dictionary<Extensions, string>
        {
            {Extensions.CSV, "66-69-6C-65-2C-66-6F-72"},
            {Extensions.Xlsx, "50-4B-03-04-14-00-06-00"}
        };

        public List<Contact> RetrieveContactsFromFile(string path)
        {
            var contacts = new List<Contact>();

            try
            {
                contacts = RetrieveContactsFromExcel(path);
            }
            catch (Exception)
            {
                try
                {
                    contacts = RetrieveContactsFromCsv(path);
                }
                catch (Exception)
                {
                    throw new FormatException("File format is not supported");
                }
            }
            finally
            {
                try
                {
                    File.Delete(path);
                }
                catch (Exception e)
                {
                    throw new FileNotFoundException(e.Message);
                }
            }
            return contacts;
        }

        private List<Contact> RetrieveContactsFromExcel(string path)
        {
            var excel = new ExcelQueryFactory(path);
            var sheets = excel.GetWorksheetNames();
            var contactsRows = (from c in excel.Worksheet<Row>(sheets.First())
                                select c).ToList();

            return contactsRows.Select(contactRow => new ContactRequestModel
            {
                FullName = contactRow["FullName"],
                CompanyName = contactRow["CompanyName"],
                Position = contactRow["Position"],
                Country = contactRow["Country"],
                Email = contactRow["Email"]
            }).Select(x => modelFactory.CreateContact(x)).ToList();
        }

        private List<Contact> RetrieveContactsFromCsv(string path)
        {
            var contacts = new List<ContactRequestModel>();
            var lines = File.ReadAllLines(path);

            for (int i = 1; i < lines.Length; i++)
            {
                var currentLine = lines[i].Split(',');
                var contact = new ContactRequestModel
                {
                    FullName = currentLine[0],
                    CompanyName = currentLine[1],
                    Position = currentLine[2],
                    Country = currentLine[3],
                    Email = currentLine[4]
                };
                contacts.Add(contact);
            }
            return contacts.Select(x => modelFactory.CreateContact(x)).ToList();
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
