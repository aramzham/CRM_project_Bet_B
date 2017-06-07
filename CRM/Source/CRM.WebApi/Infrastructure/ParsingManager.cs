using CRM.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using CRM.WebApi.Models;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace CRM.WebApi.Infrastructure
{
    public class ParsingManager
    {
        private ModelFactory modelFactory = new ModelFactory();

        private readonly Dictionary<Extensions, string> ExtensionSignature = new Dictionary<Extensions, string>
        {
            {Extensions.CSV, "66-69-6C-65-2C-66-6F-72"},
            {Extensions.Xlsx, "50-4B-03-04-14-00-06-00"},
            {Extensions.Xls, "D0-CF-11-E0-A1-B1-1A-E1"}
        };

        public List<ContactRequestModel> RetrieveContactsFromFile(byte[] bytes)
        {
            List<ContactRequestModel> contacts;
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

        private List<ContactRequestModel> ReadExcelFileDOM(string path)
        {
            var strProperties = new string[5];
            var contactRequestModels = new List<ContactRequestModel>();
            ContactRequestModel model;
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
                    model = new ContactRequestModel { FullName = strProperties[0], CompanyName = strProperties[1], Position = strProperties[2], Country = strProperties[3], Email = strProperties[4] };
                    if (strProperties.Any(x => string.IsNullOrEmpty(x)) || !new EmailAddressAttribute().IsValid(strProperties[4])) contactRequestModels.Add(null);
                    else contactRequestModels.Add(model);
                }
                //return contactRequestModels.Select(x => modelFactory.CreateContact(x)).ToList();
                return contactRequestModels;
            }
        }

        private List<ContactRequestModel> RetrieveContactsFromCsv(string path)
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
                    var contact = new ContactRequestModel
                    {
                        FullName = currentLine[fullNameIndex],
                        CompanyName = currentLine[companyNameIndex],
                        Position = currentLine[positionIndex],
                        Country = currentLine[countryIndex],
                        Email = currentLine[emailIndex]
                    };
                    if (currentLine.Any(string.IsNullOrEmpty) || !new EmailAddressAttribute().IsValid(currentLine[emailIndex])) contact = null;
                    contacts.Add(contact);
                }
                //return contacts.Select(x => modelFactory.CreateContact(x)).ToList();
                return contacts;
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
