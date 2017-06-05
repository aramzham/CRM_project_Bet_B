using CRM.EntityFramework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using CRM.WebApi.Models;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace CRM.WebApi.Infrastructure
{
    [NotImplExceptionFilter]
    public class ParsingManager
    {
        private ModelFactory modelFactory = new ModelFactory();

        private readonly Dictionary<Extensions, string> ExtensionSignature = new Dictionary<Extensions, string>
        {
            {Extensions.CSV, "66-69-6C-65-2C-66-6F-72"},
            {Extensions.Xlsx, "50-4B-03-04-14-00-06-00"},
            {Extensions.Xls, "D0-CF-11-E0-A1-B1-1A-E1"}
        };

        public List<Contact> RetrieveContactsFromFile(byte[] bytes)
        {
            var contacts = new List<Contact>();
            Extensions currentExtension = GetExtension(bytes);
            var path = HttpContext.Current?.Request.MapPath($"~//Templates//file.{currentExtension}");

            try
            {
                if (File.Exists(path)) File.Delete(path);

                File.WriteAllBytes(path, bytes);

                contacts = currentExtension == Extensions.CSV ? RetrieveContactsFromCsv(path) : ReadExcelFileDOM(path);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
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

        //private List<Contact> RetrieveContactsFromExcel(string path)
        //{
        //    var excel = new ExcelQueryFactory(path);
        //    var sheets = excel.GetWorksheetNames();
        //    var contactsRows = (from c in excel.Worksheet<Row>(sheets.First())
        //                        select c).ToList();

        //    return contactsRows.Select(contactRow => new ContactRequestModel
        //    {
        //        FullName = contactRow["FullName"],
        //        CompanyName = contactRow["CompanyName"],
        //        Position = contactRow["Position"],
        //        Country = contactRow["Country"],
        //        Email = contactRow["Email"]
        //    }).Select(x => modelFactory.CreateContact(x)).ToList();
        //}

        private List<Contact> ReadExcelFileDOM(string path)
        {
            var strProperties = new string[5];
            var contactRequestModels = new List<ContactRequestModel>();
            ContactRequestModel model = null;
            var j = 0;
            using (var myDoc = SpreadsheetDocument.Open(path, false))
            {
                var workbookPart = myDoc.WorkbookPart;
                var sheets = myDoc.WorkbookPart.Workbook.GetFirstChild<Sheets>().Elements<Sheet>();
                var relationshipId = sheets?.First().Id.Value;
                var worksheetPart = (WorksheetPart)myDoc.WorkbookPart.GetPartById(relationshipId);
                var sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();
                var i = 1;
                string value;
                foreach (var r in sheetData.Elements<Row>())
                {
                    if (i != 1)
                    {
                        foreach (var c in r.Elements<Cell>())
                        {
                            if (c == null) continue;

                            value = c.InnerText;
                            if (c.DataType != null)
                            {
                                var stringTable = workbookPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();
                                if (stringTable != null)
                                {
                                    value = stringTable.SharedStringTable.
                                      ElementAt(int.Parse(value)).InnerText;
                                }
                            }
                            strProperties[j] = value;
                            j = j + 1;
                        }
                    }
                    j = 0;
                    i = i + 1;
                    if (strProperties.Any(string.IsNullOrEmpty)) continue;
                    model = new ContactRequestModel { FullName = strProperties[0], CompanyName = strProperties[1], Position = strProperties[2], Country = strProperties[3], Email = strProperties[4] };
                    contactRequestModels.Add(model);
                }
                return contactRequestModels.Select(x => modelFactory.CreateContact(x)).ToList();
            }
        }

        private List<Contact> RetrieveContactsFromCsv(string path)
        {
            var contacts = new List<ContactRequestModel>();
            string[] lines;
            try
            {
                lines = File.ReadAllLines(path);
                if (lines.Length == 0) return null;

                var columnNames = lines[0].Split(',');
                var fullNameIndex = Array.IndexOf(columnNames, "FullName");
                var companyNameIndex = Array.IndexOf(columnNames, "CompanyName");
                var positionIndex = Array.IndexOf(columnNames, "Position");
                var countryIndex = Array.IndexOf(columnNames, "Country");
                var emailIndex = Array.IndexOf(columnNames, "Email");
                if (new int[] { fullNameIndex, companyNameIndex, positionIndex, countryIndex, emailIndex }.Any(x => x == -1)) return null;

                for (int i = 1; i < lines.Length; i++)
                {
                    if (string.IsNullOrEmpty(lines[i])) continue;
                    var currentLine = lines[i].Split(',');
                    if (currentLine.Any(string.IsNullOrEmpty)) continue;
                    var contact = new ContactRequestModel
                    {
                        FullName = currentLine[fullNameIndex],
                        CompanyName = currentLine[companyNameIndex],
                        Position = currentLine[positionIndex],
                        Country = currentLine[countryIndex],
                        Email = currentLine[emailIndex]
                    };
                    contacts.Add(contact);
                }
                return contacts.Select(x => modelFactory.CreateContact(x)).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }

        private enum Extensions
        {
            CSV,
            Xlsx,
            Xls
        }

        private Extensions GetExtension(byte[] bytes)
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
                case Extensions.Xls:
                    return extension;
            }
            throw new FormatException("The format of uploaded file was incorrect. Only .csv and MS Excel supported files are allowed.");
        }
    }
}
