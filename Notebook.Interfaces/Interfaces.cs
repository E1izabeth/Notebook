using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notebook.Interfaces
{

    public interface IContactInfo
    {
        string FirstName { get; }
        string LastName { get; }
        string Nickname { get; }
        string Birthday { get; }
        string Phone { get; }
        string Email { get; }
        string Mailer { get; }
        string Note { get; }
    }


    public interface INotebook
    {
        // List<Contact> List { get; set; }
        void NewElement(IContactInfo contact);

        IEnumerable<IContactInfo> GetContacts();
        IEnumerable<IContactInfo> GetContacts(SearchSpec spec);
        //void Loader(string _path);
    }   
}
