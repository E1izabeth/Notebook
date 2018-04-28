using Notebook.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using VCard;

namespace WpfUI
{
    public class AppConnector
    {
        readonly INotebook _book;

        public AppConnector(INotebook nb)
        {
            _book = nb;
        }

        public bool Ping()
        {
            try
            {
                var ping = _book.Ping();
            }
            catch(CommunicationException)
            {
                return false;
            }
            return true;
        }

        public IEnumerable<IContactInfo> ViewAll()
        {
            var arr = _book.GetContacts();
            return arr.ToList<IContactInfo>();
        }

        public void AddNew(ContactInfo contact)
        {
            _book.NewElement(contact);
            SaveNewContact(contact);
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



        public List<IContactInfo> NameSearch(string searchText)
        {
            var contactInfos = _book.GetContacts(specName: new ByNameSearchCriteria(searchText));
            return contactInfos.ToList<IContactInfo>();
        }

        public List<IContactInfo> SurnameSearch(string searchText)
        {
            var contactInfos = _book.GetContacts(specSurname: new BySurnameSearchCriteria(searchText));
            return contactInfos.ToList<IContactInfo>();
        }

        public List<IContactInfo> PhoneSearch(string searchText)
        {
            var contactInfos = _book.GetContacts(specPhone: new ByPhoneSearchCriteria(searchText));
            return contactInfos.ToList<IContactInfo>();
        }

        public List<IContactInfo> EmailSearch(string searchText)
        {
            var contactInfos = _book.GetContacts(specEmail: new ByEmailSearchCriteria(searchText));
            return contactInfos.ToList<IContactInfo>();
        }

        public List<IContactInfo> NameSurnameSearch(string searchText)
        {
            var contactInfos = _book.GetContacts(specName: new ByNameSearchCriteria(searchText), specSurname: new BySurnameSearchCriteria(searchText));
            return contactInfos.ToList<IContactInfo>();
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
