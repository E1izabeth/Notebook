using Notebook.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VCard;

namespace WpfUI
{
    class AppConnector
    {
        readonly INotebook _book;

        public AppConnector(INotebook nb)
        {
            _book = nb;
        }

        private List<string> ConvertListOfIContactInfosToListOfString(List<IContactInfo> contactInfos)
        {
            var strArr = new List<string>();
            foreach (var item in contactInfos)
            {
                strArr.Add($"Name: {item.FirstName} {item.LastName}\n Phone: {item.Phone} Email: {item.Email} Mailer: {item.Mailer} \n Birthday: {item.Birthday} Note: {item.Note}");
            }
            return strArr;
        }

        public List<string> ViewAll()
        {
            var arr = _book.GetContacts();
            return ConvertListOfIContactInfosToListOfString(arr.ToList<IContactInfo>());
        }

        public void AddNew()
        {
            throw new NotImplementedException();
            //var NewContact = _user.AddNew();
            //_book.NewElement(NewContact);
            //SaveNewContact(NewContact);
        }

        public void LoadNew(string path)
        {
            using (var stream = File.OpenRead(path))
            {
                using (var writer = new StreamReader(stream))
                {
                    VCardFile NewFile = VCardFile.ReadFrom(writer);
                    foreach (VCardEntry entry in NewFile.Entries)
                    {
                        var nameLine = entry.Contents.FirstOrDefault(l => l.Name == Names.N);
                        var nameParts = nameLine?.Value.Split(new[] { ';' }, 2);
                        var lastName = nameParts?.FirstOrDefault();
                        var firstName = nameParts?.Length < 2 ? string.Empty : nameParts.Last();
                        ContactInfo contact = new ContactInfo {
                            FirstName = firstName,
                            LastName = lastName,
                            Nickname = entry.Contents.FirstOrDefault(l => l.Name == Names.NICKNAME)?.Value,
                            Birthday = entry.Contents.FirstOrDefault(l => l.Name == Names.BDAY)?.Value,
                            Phone = entry.Contents.FirstOrDefault(l => l.Name == Names.TEL)?.Value,
                            Email = entry.Contents.FirstOrDefault(l => l.Name == Names.EMAIL)?.Value,
                            Mailer = entry.Contents.FirstOrDefault(l => l.Name == Names.MAILER)?.Value,
                            Note = entry.Contents.FirstOrDefault(l => l.Name == Names.NOTE)?.Value
                        };
                        _book.NewElement(contact);
                    }
                }
            }

        }



        public List<string> NameSearch(string searchText)
        {
            var contactInfos = _book.GetContacts(specName: new ByNameSearchCriteria(searchText));
            return ConvertListOfIContactInfosToListOfString(contactInfos.ToList<IContactInfo>());
        }

        public List<string> SurnameSearch(string searchText)
        {
            var contactInfos = _book.GetContacts(specSurname: new BySurnameSearchCriteria(searchText));
            return ConvertListOfIContactInfosToListOfString(contactInfos.ToList<IContactInfo>());
        }

        public List<string> PhoneSearch(string searchText)
        {
            var contactInfos = _book.GetContacts(specPhone: new ByPhoneSearchCriteria(searchText));
            return ConvertListOfIContactInfosToListOfString(contactInfos.ToList<IContactInfo>());
        }

        public List<string> EmailSearch(string searchText)
        {
            var contactInfos = _book.GetContacts(specEmail: new ByEmailSearchCriteria(searchText));
            return ConvertListOfIContactInfosToListOfString(contactInfos.ToList<IContactInfo>());
        }

        public void NameSurnameSearch()
        {
            throw new NotImplementedException();
            //string query = _user.AskRequest();
            //var contactInfos = _book.GetContacts(specName: new ByNameSearchCriteria(query), specSurname: new BySurnameSearchCriteria(query));
            //_user.PrintSelection(contactInfos);
        }

        private void SaveNewContact(IContactInfo NewContact)
        {
            var file = new VCardFile(
                new VCardEntry(
                    string.Empty,
                    new ContentLine(Names.VERSION, "3.0"),
                    new ContentLine(Names.N, NewContact.LastName + ";" + NewContact.FirstName),
                    new ContentLine(Names.FN, NewContact.LastName + " " + NewContact.FirstName),
                    new ContentLine(Names.NICKNAME, NewContact.Nickname),
                    new ContentLine(Names.BDAY, NewContact.Birthday),
                    new ContentLine(Names.TEL, NewContact.Phone),
                    new ContentLine(Names.EMAIL, NewContact.Email),
                    new ContentLine(Names.MAILER, NewContact.Mailer),
                    new ContentLine(Names.NOTE, NewContact.Note)
                )
            );

            //var e = file.Entries.First();
            //var rawName = e.Contents.FirstOrDefault(c => c.Name == Names.N);
            //var name = rawName.Value.Split(';');

            using (var stream = File.OpenWrite(@"D:/contacts/" + _book.Count() + "_contact.vcf"))
            using (var writer = new StreamWriter(stream))
            {
                file.WriteTo(writer);
            }
        }

        public void SaveAllContacts(string path)
        {
            var file = new VCardFile();
            foreach (var Tmp in _book.GetContacts().ToList())
            {
                file.Entries.Add(
                    new VCardEntry(
                            string.Empty,
                            new ContentLine(Names.VERSION, "3.0"),
                            new ContentLine(Names.N, Tmp.LastName + ";" + Tmp.FirstName),
                            new ContentLine(Names.FN, Tmp.LastName + " " + Tmp.FirstName),
                            new ContentLine(Names.NICKNAME, Tmp.Nickname),
                            new ContentLine(Names.BDAY, Tmp.Birthday),
                            new ContentLine(Names.TEL, Tmp.Phone),
                            new ContentLine(Names.EMAIL, Tmp.Email),
                            new ContentLine(Names.MAILER, Tmp.Mailer),
                            new ContentLine(Names.NOTE, Tmp.Note)
                        )
                );
            }
            using (var stream = File.OpenWrite($"{path}/{new Regex("[.|:| ]").Replace(DateTime.Today.ToString(), "")}_nb.vcf"))
            using (var writer = new StreamWriter(stream))
            {
                file.WriteTo(writer);
            }
        }
    }
}
