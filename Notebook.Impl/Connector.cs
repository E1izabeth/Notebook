using Notebook.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace Notebook.Impl
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class Connector : INotebook
    {
        private readonly NotebookImpl _notebook;

        public Connector()
        {
            _notebook = new NotebookImpl();
        }

        private static List<ContactInfo> ConvertListOfIContactInfoToListOfContactInfo(List<IContactInfo> list)
        {
            var newList = new List<ContactInfo>(list.Count);
            foreach (var item in list)
            {
                var cnt = ConvertIContactInfoToContactInfo(item);
                newList.Add(cnt);
            }
            return newList;
        }

        private static ContactInfo ConvertIContactInfoToContactInfo(IContactInfo contact)
        {
            var cnt = new ContactInfo {
                FirstName = contact.FirstName,
                LastName = contact.LastName,
                Birthday = contact.Birthday,
                Nickname = contact.Nickname,
                Email = contact.Email,
                Phone = contact.Phone,
                Mailer = contact.Mailer,
                Note = contact.Note
            };
            return cnt;
        }

        public int Count()
        {
            return _notebook.Count();
        }

        public ContactInfo[] GetContacts()
        {
            var arr = _notebook.GetContacts().ToList<IContactInfo>();
            return ConvertListOfIContactInfoToListOfContactInfo(arr).ToArray<ContactInfo>();
        }

        public void NewElement(ContactInfo contact)
        {
            _notebook.NewElement(contact);
        }

        public DateTime Ping()
        {
            return _notebook.Ping();
        }

        //public ContactInfo[] GetContacts(SearchSpec spec)
        //{
        //    var arr = _notebook.GetContacts(spec).ToList<IContactInfo>();
        //    return ConvertListOfIContactInfoToListOfContactInfo(arr).ToArray<ContactInfo>();
        //}

        public ContactInfo[] GetContacts(ByNameSearchCriteria specName = null, BySurnameSearchCriteria specSurname = null, 
                                            ByPhoneSearchCriteria specPhone = null, ByEmailSearchCriteria specEmail = null)
        {
            var arr = _notebook.GetContacts(new SearchSpec(specName, specSurname, specPhone, specEmail)).ToList();
            return ConvertListOfIContactInfoToListOfContactInfo(arr).ToArray<ContactInfo>();
        }
    }
}
