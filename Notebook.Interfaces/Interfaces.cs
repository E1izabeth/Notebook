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
        string Name { get; }
        string Surname { get; }
        string Phone { get; }
        string Email { get; }
    }


    public interface INotebook
    {
        // List<Contact> List { get; set; }
        void NewElement(IContactInfo contact);

        IEnumerable<IContactInfo> GetContacts();
        IEnumerable<IContactInfo> GetContacts(SearchSpec spec);
    }
}
