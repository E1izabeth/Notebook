using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Notebook.Interfaces;
using System.IO;
using VCard;
using System.Net.Sockets;
using System.Net;

namespace Notebook
{
    class App
    {
        readonly Dialogs _user = new Dialogs();
        readonly INotebook _book;

        public App(INotebook nb)
        {
            _book = nb;
        }

        void ViewAll()
        {
            _user.PrintSelection(_book.GetContacts().ToList());
        }

        void AddNew()
        {
            var NewContact = _user.AddNew();
            _book.NewElement(NewContact);
            SaveNewContact(NewContact);
        }

        public void CheckValidPath(string path)
        {

            if (!(File.Exists(path)) || !(path.Contains(".vcf")))
            {
                Console.WriteLine("Please, input valid path");
                LoadNew();
            }
        }
        
        void LoadNew()
        {
            string path = _user.LoadPath();
            CheckValidPath(path);
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
                        /*
                        var s = Console.ReadLine();
                        var x = s ?? string.Empty;
                        var t = s != null ? s : string.Empty ;
                         */
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

        void NameSearch()
        {
            string query = _user.AskRequest();
            //Request(Contact => Contact.Name.Contains(query));
            this.PerformQuery(new ByNameSearchCriteria(query));
        }

        void SurnameSearch()
        {
            string query = _user.AskRequest();
            //Request(Contact => Contact.Surname.Contains(query));
            this.PerformQuery(new BySurnameSearchCriteria(query));
        }

        void PhoneSearch()
        {
            string query = _user.AskRequest();
            //Request(Contact => Contact.Phone.Contains(query));
            this.PerformQuery(new ByPhoneSearchCriteria(query));
        }

        void EmailSearch()
        {
            string query = _user.AskRequest();
            //Request(Contact => Contact.Email.Contains(query));
            this.PerformQuery(new ByEmailSearchCriteria(query));
        }

        void NameSurnameSearch()
        {
            string query = _user.AskRequest();
            //Request(Contact => Contact.Name.Contains(query) || Contact.Surname.Contains(query));
            this.PerformQuery(new ByNameSearchCriteria(query), new BySurnameSearchCriteria(query));
        }

        void PerformQuery(params SearchCriteria[] args)
        {
            _user.PrintSelection(_book.GetContacts(new SearchSpec(args)).ToList());
        }


        /*void Request(Predicate<Contact> Selection)
        {
            _user.PrintSelection(_book.List.FindAll(Selection));
        }*/

        static void Main(string[] argc)
        {
            var app = new App(Notebook.Impl.Notebooks.CreateRemoteNotebook(Int32.Parse(argc[0])));

            var menu = new Menu(
                new MenuItem("Menu") {
                    new MenuItem("View all", app.ViewAll),                                      
                    new MenuItem("Load new", app.LoadNew),
                    new MenuItem("Add New", app.AddNew ),                                      
                    new MenuItem("Search") {
                        new MenuItem("By name", app.NameSearch),
                        new MenuItem("By surname", app.SurnameSearch),
                        new MenuItem("Be name and surname", app.NameSurnameSearch),
                        new MenuItem("By phone", app.PhoneSearch),
                        new MenuItem("By E-mail", app.EmailSearch),
                    },
                    new MenuItem("Upload All", app.SaveAllContacts)
                }
            );

            menu.RunMenu();
        }

        private void SaveNewContact(IContactInfo NewContact)
        {
            //var rnd = new Random();
            //var allContacts = _book.GetContacts().ToArray();
            //var contact = allContacts[rnd.Next(allContacts.Length)];

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

        void SaveAllContacts()
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
            using (var stream = File.OpenWrite(@"D:/contacts/Notebook.vcf"))
            using (var writer = new StreamWriter(stream))
            {
                file.WriteTo(writer);
            }
        }
    }
}