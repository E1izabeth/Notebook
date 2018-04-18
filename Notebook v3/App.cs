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
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Tcp;
using System.Threading;
using System.Globalization;

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
            var arr = _book.GetContacts();
            _user.PrintSelection(arr.ToList<IContactInfo>());
        }

        void AddNew()
        {
            var NewContact = _user.AddNew();
            _book.NewElement(NewContact);
            //SaveNewContact(NewContact);
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
                        //throw new NotImplementedException("");
                    }
                }
            }

        }

        void NameSearch()
        {
            string query = _user.AskRequest();
            //Request(Contact => Contact.Name.Contains(query));
            var contactInfos = _book.GetContacts(specName: new ByNameSearchCriteria(query));
            _user.PrintSelection(contactInfos);
            //this.PerformQuery(new ByNameSearchCriteria(query));
        }

        void SurnameSearch()
        {
            string query = _user.AskRequest();
            //Request(Contact => Contact.Surname.Contains(query));
            var contactInfos = _book.GetContacts(specSurname: new BySurnameSearchCriteria(query));
            _user.PrintSelection(contactInfos);
            //this.PerformQuery(new BySurnameSearchCriteria(query));
        }

        void PhoneSearch()
        {
            string query = _user.AskRequest();
            //Request(Contact => Contact.Phone.Contains(query));
            var contactInfos = _book.GetContacts(specPhone: new ByPhoneSearchCriteria(query));
            _user.PrintSelection(contactInfos);
            //this.PerformQuery(new ByPhoneSearchCriteria(query));
        }

        void EmailSearch()
        {
            string query = _user.AskRequest();
            //Request(Contact => Contact.Email.Contains(query));
            var contactInfos = _book.GetContacts(specEmail: new ByEmailSearchCriteria(query));
            _user.PrintSelection(contactInfos);
            //this.PerformQuery(new ByEmailSearchCriteria(query));
        }

        void NameSurnameSearch()
        {
            string query = _user.AskRequest();
            //Request(Contact => Contact.Name.Contains(query) || Contact.Surname.Contains(query));
            var contactInfos = _book.GetContacts(specName: new ByNameSearchCriteria(query), specSurname: new BySurnameSearchCriteria(query));
            _user.PrintSelection(contactInfos);
            //this.PerformQuery(new ByNameSearchCriteria(query), new BySurnameSearchCriteria(query));
        }

        //void PerformQuery(params SearchCriteria[] args)
        //{
        //    var searchParams = new SearchSpec(args);
        //    var contactInfos = _book.GetContacts(searchParams);
        //    _user.PrintSelection(contactInfos);
        //}

        /*void Request(Predicate<Contact> Selection)
        {
            _user.PrintSelection(_book.List.FindAll(Selection));
        }*/

        static void Main(string[] argc)
        {
            AppDomain.CurrentDomain.FirstChanceException += (sender, ea) => {
                System.Diagnostics.Debug.Print(System.Diagnostics.Process.GetCurrentProcess().ProcessName + ": " + ea.Exception.ToString());
            };

            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("ru-RU");
            //var app = new App(Client.Client.SvcClient(Int32.Parse(argc[0])));
            //var app = new App(new Client.Client(Int32.Parse(argc[0])));
            var app = new App(Client.Client.WCFclient());

            var menu = new Menu(
                new MenuItem(Notebook_v3.Properties.text.Menu) {
                    new MenuItem(Notebook_v3.Properties.text.View_all, app.ViewAll),
                    new MenuItem(Notebook_v3.Properties.text.Load_new, app.LoadNew),
                    new MenuItem(Notebook_v3.Properties.text.Add_new, app.AddNew ),
                    new MenuItem(Notebook_v3.Properties.text.Search) {
                        new MenuItem(Notebook_v3.Properties.text.By_name, app.NameSearch),
                        new MenuItem(Notebook_v3.Properties.text.By_surname, app.SurnameSearch),
                        new MenuItem(Notebook_v3.Properties.text.By_name_surname, app.NameSurnameSearch),
                        new MenuItem(Notebook_v3.Properties.text.By_phone, app.PhoneSearch),
                        new MenuItem(Notebook_v3.Properties.text.By_email, app.EmailSearch),
                    },
                    new MenuItem(Notebook_v3.Properties.text.Upload_all, app.SaveAllContacts)
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