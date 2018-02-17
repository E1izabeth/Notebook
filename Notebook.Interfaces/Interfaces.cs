using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using System.Runtime.Serialization;

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

    //[DataContract]
    //public class Contacts
    //{

    //    [DataMember]
    //    public IContactInfo[] Items { get; set; }

    //    public Contacts()
    //    {

    //    }
    //}

    [ServiceContract()]
    public interface INotebook
    {
        [OperationContract(Name = "NewContact")]
        void NewElement(ContactInfo contact);

        [OperationContract()]
        [WebGet(UriTemplate = "/contacts?action=getContacts")]
        ContactInfo[] GetContacts();


        [OperationContract(Name = "SearchContacts")]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "/contacts?action=searchCount")]
        ContactInfo[] GetContacts(ByNameSearchCriteria specName = null, BySurnameSearchCriteria specSurname = null,
                                    ByPhoneSearchCriteria specPhone = null, ByEmailSearchCriteria specEmail = null);

        [OperationContract(Name = "Count")]
        [WebGet(UriTemplate = "/contacts?action=getCount")]
        int Count();
    }



    public interface INotebookLocal
    {
        // List<Contact> List { get; set; }
       
        void NewElement(IContactInfo contact);

        IEnumerable<IContactInfo> GetContacts();

        IEnumerable<IContactInfo> GetContacts(SearchSpec spec);

        int Count();
    }
}
