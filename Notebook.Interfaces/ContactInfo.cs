using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Notebook.Interfaces
{
    [Serializable]
    [DataContract]
    public class ContactInfo : IContactInfo
    {
        [DataMember]
        public string FirstName { get; set; }
        [DataMember]
        public string LastName { get; set; }
        [DataMember]
        public string Nickname { get; set; }
        [DataMember]
        public string Birthday { get; set; }
        [DataMember]
        public string Phone { get; set; }
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public string Mailer { get; set; }
        [DataMember]
        public string Note { get; set; }

        public ContactInfo()
        {

        }
    }
}
