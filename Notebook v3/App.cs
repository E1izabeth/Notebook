using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Notebook.Interfaces;
using System.IO;

namespace Notebook_v3
{
    class App
    {
        readonly Dialogs _user = new Dialogs();
        readonly INotebook _book = Notebook.Impl.Notebooks.CreateLocalNotebook();

        public App()
        {
        }

        void ViewAll()
        {
            _user.PrintSelection(_book.GetContacts().ToList());
        }

        void AddNew()
        {
            _book.NewElement(_user.AddNew());
        }

        void LoadNew()
        {
            _book.Loader(_user.LoadPath());
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

        static void Main(string[] args)
        {
            var app = new App();

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
                    }
                }
            );

            menu.RunMenu();
        }
    }
}
