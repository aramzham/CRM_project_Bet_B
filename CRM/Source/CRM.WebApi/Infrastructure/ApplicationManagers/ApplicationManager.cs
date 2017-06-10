using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using CRM.EntityFramework;
using CRM.WebApi.Models;

namespace CRM.WebApi.Infrastructure.ApplicationManagers
{
    public class ApplicationManager : IDisposable
    {
        private CRMDatabaseEntities db = new CRMDatabaseEntities();
        private ModelFactory modelFactory = new ModelFactory();
        #region Contacts methods
        public async Task<List<ContactResponseModel>> GetAllContacts()
        {
            var listOfContacts = await db.Contacts.ToListAsync();
            return listOfContacts?.Select(modelFactory.CreateContactResponseModel).ToList();
        }
        public async Task<ContactResponseModel> GetContactByGuid(string guid)
        {
            var contact = await db.Contacts.FirstOrDefaultAsync(x => x.Guid.ToString() == guid);
            return contact == null ? null : modelFactory.CreateContactResponseModel(contact);
        }

        //public async Task<List<Contact>> GetByPage(int start, int numberOfRows, bool @ascending)
        //{
        //    if (ascending) return await db.Contacts.OrderBy(x => x.ID).Skip(start - 1).Take(numberOfRows).ToListAsync();
        //    else return await db.Contacts.OrderByDescending(x => x.ID).Skip(start - 1).Take(numberOfRows).ToListAsync();
        //}

        public async Task<bool> UpdateContact(string guid, ContactRequestModel contact)
        {
            var contactToUpdate = await db.Contacts.FirstOrDefaultAsync(x => x.Guid.ToString() == guid);
            if (contactToUpdate != null)
            {
                if (contact.FullName != null) contactToUpdate.FullName = contact.FullName;
                if (contact.CompanyName != null) contactToUpdate.CompanyName = contact.CompanyName;
                if (contact.Position != null) contactToUpdate.Position = contact.Position;
                if (contact.Country != null) contactToUpdate.Country = contact.Country;
                if (contact.Email != null && contactToUpdate.Email != contact.Email)
                {
                    if (!(await GetAllEmails()).Contains(contact.Email)) contactToUpdate.Email = contact.Email;
                    else return false;
                }
                contactToUpdate.DateModified = DateTime.UtcNow;
                db.Entry(contactToUpdate).State = EntityState.Modified;
            }

            try
            {
                await db.SaveChangesAsync();
            }
            catch
            {
                if (!await ContactExists(guid)) return false;
                else throw;
            }
            return true;
        }
        public async Task<ContactResponseModel> AddContact(ContactRequestModel requestContact)
        {
            var contactToAdd = modelFactory.CreateContact(requestContact);
            db.Contacts.Add(contactToAdd);
            await db.SaveChangesAsync();

            return modelFactory.CreateContactResponseModel(contactToAdd);
        }

        public async Task<bool> AddMultipleContacts(List<ContactRequestModel> contacts)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    db.Contacts.AddRange(contacts.Where(x => x != null).Select(x => modelFactory.CreateContact(x)));
                    await db.SaveChangesAsync();
                    transaction.Commit();
                    return true;
                }
                catch
                {
                    transaction.Rollback();
                    return false;
                }
            }
        }

        public async Task<ContactResponseModel> RemoveContact(string guid)
        {
            if (string.IsNullOrEmpty(guid)) return null;
            var contact = await db.Contacts.Where(x => x.Guid.ToString() == guid).FirstOrDefaultAsync();
            if (contact == null) return null;

            db.Contacts.Remove(contact);
            await db.SaveChangesAsync();

            return modelFactory.CreateContactResponseModel(contact);
        }

        public async Task<List<ContactResponseModel>> RemoveContactByGroup(string[] guids)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var listOfRemovedContacts = new List<ContactResponseModel>();
                    ContactResponseModel contactResponseModel;
                    foreach (var guid in guids)
                    {
                        contactResponseModel = await RemoveContact(guid);
                        if (contactResponseModel == null) throw new Exception();
                        listOfRemovedContacts.Add(contactResponseModel);
                    }
                    transaction.Commit();
                    return listOfRemovedContacts;
                }
                catch
                {
                    transaction.Rollback();
                    return null;
                }
            }
        }

        public async Task<bool> ContactExists(string guid)
        {
            return await db.Contacts.CountAsync(e => e.Guid.ToString() == guid) > 0;
        }

        public async Task<List<string>> GetAllEmails()
        {
            return await db.Contacts.Select(x => x.Email).ToListAsync();
        }

        public async Task<string> ResetForDemo()
        {
            var group1 = new List<ContactResponseModel>();
            var group2 = new List<ContactResponseModel>();
            var group3 = new List<ContactResponseModel>();
            try
            {
                await db.Database.ExecuteSqlCommandAsync("delete from Contacts");
                await db.Database.ExecuteSqlCommandAsync("delete from MailingLists");
                await db.SaveChangesAsync();
                var test1 = new ContactRequestModel { FullName = "Aram Zhamkochyan", CompanyName = "BetConstruct", Position = "Intern", Country = "Armenia", Email = "aram532@yandex.ru" };
                var test2 = new ContactRequestModel { FullName = "Lusine Khachatryan", CompanyName = "BetConstruct", Position = "Intern", Country = "Armenia", Email = "luskhachatryann@gmail.com" };
                var test3 = new ContactRequestModel { FullName = "Sargis Chilingaryan", CompanyName = "BetConstruct", Position = "Front-end developer", Country = "Armenia", Email = "1995saqo@mail.ru" };
                var test4 = new ContactRequestModel { FullName = "Davit Hunanyan", CompanyName = "STDev", Position = "Frontend developer", Country = "Armenia", Email = "0777dav@gmail.com" };
                var test5 = new ContactRequestModel { FullName = "Vahan Kalenteryan", CompanyName = "BetConstruct", Position = "Front-end developer", Country = "Armenia", Email = "vahan.kalenteryan@gmail.com" };
                var test6 = new ContactRequestModel { FullName = "Gayane Khachatryan", CompanyName = "BetConstruct", Position = "Back-end developer", Country = "Armenia", Email = "gayane.jane@gmail.com" };
                var test7 = new ContactRequestModel { FullName = "Narek Yegoryan", CompanyName = "BetContstruct", Position = "Back-end developer", Country = "Armenia", Email = "yegoryan.narek@gmail.com" };
                var test8 = new ContactRequestModel { FullName = "Aghasi Lorsabyan", CompanyName = "TUMO", Position = "Trainer", Country = "Kharabakh", Email = "lorsabyan@gmail.com" };
                var test9 = new ContactRequestModel { FullName = "Khachatur Sukiasyan", CompanyName = "SIL Group", Position = "Owner", Country = "Armenia", Email = "khachatur125@gmail.com" };
                var test10 = new ContactRequestModel { FullName = "Meri Sahakyan", CompanyName = "BetConstruct", Position = "Intern", Country = "Armenia", Email = "sahakyan_m@bk.ru" };
                var test11 = new ContactRequestModel { FullName = "Van Hakobyan", CompanyName = "BetConstruct", Position = "Intern", Country = "Armenia", Email = "vanhakobyan1996@gmail.com" };
                var test12 = new ContactRequestModel { FullName = "Hovhanes Nalbandyan", CompanyName = "BetConstruct", Position = "Developer", Country = "Armenia", Email = "nalbandyan.1991@bk.ru" };
                var test13 = new ContactRequestModel { FullName = "Lusine Hovsepyan", CompanyName = "MIC Armenia", Position = "Developer", Country = "Armenia", Email = "lusine@hovsepyan.am" };
                var test14 = new ContactRequestModel { FullName = "Tsovinar Ghazaryan", CompanyName = "MIC Armenia", Position = "Back-end developer", Country = "Armenia", Email = "tsovinar.ghazaryan@yahoo.com" };
                foreach (var testContact in new[] { test1, test2, test4, test5, test6 })
                    group1.Add(await AddContact(testContact));
                foreach (var testContact in new[] { test3, test7, test10, test11, test12, test13 })
                    group2.Add(await AddContact(testContact));
                foreach (var testContact in new[] { test8, test9, test14 })
                    group3.Add(await AddContact(testContact));

                var testMailingList1 = await AddMailingList("OurGroup");
                var testMailingList2 = await AddMailingList("MIC Armenia");
                var testMailingList3 = await AddMailingList("VIP");
                var ml1 = await AddContactsToMailingLists(testMailingList1.MailingListId, group1.Select(x => x.Guid.ToString()).ToArray());
                var ml2 = await AddContactsToMailingLists(testMailingList2.MailingListId, group2.Select(x => x.Guid.ToString()).ToArray());
                var ml3 = await AddContactsToMailingLists(testMailingList3.MailingListId, group3.Select(x => x.Guid.ToString()).ToArray());
                if (ml1 == null || ml2 == null || ml3 == null) return null;
                await db.SaveChangesAsync();
                var path = HttpContext.Current?.Request.MapPath("~//Templates//Demo page.html");
                return File.ReadAllText(path);
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<ContactResponseModel>> Query(string[] sorts, QueryRequestModel qrm)
        {
            var fullName = !string.IsNullOrEmpty(qrm.FullName) ? qrm.FullName : "%";
            var companyName = !string.IsNullOrEmpty(qrm.CompanyName) ? qrm.CompanyName : "%";
            var position = !string.IsNullOrEmpty(qrm.Position) ? qrm.Position : "%";
            var country = !string.IsNullOrEmpty(qrm.Country) ? qrm.Country : "%";
            var email = !string.IsNullOrEmpty(qrm.Email) ? qrm.Email : "%";
            var queryString = "select * from Contacts " +
                              $"where fullname like '{fullName}' " +
                              $"and companyname like '{companyName}' " +
                              $"and position like '{position}' " +
                              $"and country like '{country}' " +
                              $"and email like '{email}'";

            if (sorts != null && sorts.Length != 0) // order by
            {
                queryString += " order by ";
                queryString = sorts.Aggregate(queryString, (current, sort) => current + $"{sort.Replace('_', ' ')},");
                queryString = queryString.TrimEnd(',');
            }
            var rows = await db.Contacts.SqlQuery(queryString).ToListAsync();
            return rows.Select(x => modelFactory.CreateContactResponseModel(x)).ToList();
        }

        #endregion
        #region Mailing lists methods
        public async Task<List<MailingListResponseModel>> GetAllMailingLists()
        {
            var mailingLists = await db.MailingLists.ToListAsync();
            return mailingLists?.Select(x => modelFactory.CreateMailingListResponseModel(x)).ToList();
        }

        public async Task<MailingListResponseModel> GetMailingListById(int id)
        {
            var mailingList = await db.MailingLists.FirstOrDefaultAsync(x => x.ID == id);
            return mailingList == null ? null : modelFactory.CreateMailingListResponseModel(mailingList);
        }

        public async Task<MailingListResponseModel> AddMailingList(string mailingListName)
        {
            var mailingListToAdd = modelFactory.CreateMailingList(mailingListName);
            db.MailingLists.Add(mailingListToAdd);
            await db.SaveChangesAsync();

            return modelFactory.CreateMailingListResponseModel(mailingListToAdd);
        }

        public async Task<MailingListResponseModel> RemoveContactsFromMailingList(int id, string[] guids)
        {
            var mailingList = await db.MailingLists.FindAsync(id);
            if (mailingList == null) return null; //id is bad

            using (var transaction = db.Database.BeginTransaction())
            {
                Contact contact;
                try
                {
                    foreach (var guid in guids)
                    {
                        contact = await db.Contacts.FirstOrDefaultAsync(x => x.Guid.ToString() == guid);
                        if (contact == null) throw new Exception("One or more guids were corrupt"); //guid is bad
                        if (mailingList.Contacts.Contains(contact)) mailingList.Contacts.Remove(contact);
                    }
                    db.Entry(mailingList).State = EntityState.Modified;

                    try
                    {
                        await db.SaveChangesAsync();
                        transaction.Commit();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!await MailingListExists(id)) return null; //something is bad :)
                        else throw;
                    }
                    return modelFactory.CreateMailingListResponseModel(mailingList);
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    return null;
                }
            }
        }
        public async Task<MailingListResponseModel> AddContactsToMailingLists(int id, string[] guids)
        {
            if (!await MailingListExists(id)) return null;
            var mailingList = await db.MailingLists.FindAsync(id);

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    Contact contact;
                    foreach (var guid in guids)
                    {
                        contact = await db.Contacts.FirstOrDefaultAsync(x => x.Guid.ToString() == guid);
                        if (contact == null) throw new Exception("One or more guids were corrupt");
                        if (!mailingList.Contacts.Contains(contact)) mailingList.Contacts.Add(contact);
                    }
                    db.Entry(mailingList).State = EntityState.Modified;

                    try
                    {
                        await db.SaveChangesAsync();
                        transaction.Commit();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!await MailingListExists(id)) return null;
                        else throw;
                    }
                    return modelFactory.CreateMailingListResponseModel(mailingList);
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    return null;
                }
            }
        }

        public async Task<MailingListResponseModel> RenameMailingList(int id, string name)
        {
            var mailingList = await db.MailingLists.FindAsync(id);
            if (mailingList == null) return null;

            mailingList.MailingListName = name;
            db.Entry(mailingList).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await MailingListExists(id)) return null;
                else throw;
            }
            return modelFactory.CreateMailingListResponseModel(mailingList);
        }

        public async Task<MailingListResponseModel> RemoveMailingList(int id)
        {
            var mailingList = await db.MailingLists.FindAsync(id);
            if (mailingList == null) return null;

            db.MailingLists.Remove(mailingList);
            await db.SaveChangesAsync();

            return modelFactory.CreateMailingListResponseModel(mailingList);
        }

        public async Task<List<MailingListResponseModel>> RemoveSeveralMailingLists(int[] ids)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var distinctIds = ids.Distinct().ToArray();
                    var listOfRemovedLists = new List<MailingListResponseModel>();
                    foreach (var id in distinctIds)
                    {
                        if (!await MailingListExists(id)) throw new Exception();
                        var mailingResponseList = await RemoveMailingList(id);
                        await db.SaveChangesAsync();
                        listOfRemovedLists.Add(mailingResponseList);
                    }
                    transaction.Commit();
                    return listOfRemovedLists;
                }
                catch
                {
                    transaction.Rollback();
                    return null;
                }
            }
        }

        public async Task<bool> MailingListExists(int id)
        {
            return await db.MailingLists.CountAsync(e => e.ID == id) > 0;
        }
        #endregion
        #region Templates methods
        public async Task<List<TemplateResponseModel>> GetAllTemplates()
        {
            var templates = await db.Templates.ToListAsync();
            return templates?.Select(modelFactory.CreateTemplateResponseModel).ToList();
        }

        public async Task<bool> TemplateExists(int id)
        {
            return await db.Templates.CountAsync(e => e.Id == id) > 0;
        }

        #endregion
        public void Dispose()
        {
            db.Dispose();
        }
    }
}