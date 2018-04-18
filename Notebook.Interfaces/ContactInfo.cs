using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        public ContactInfo(string firstName, string lastName, string nickname, string birthday, string phone, string email, string mailer, string note)
        {
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Nickname = nickname;
            this.Birthday = birthday;
            this.Phone = phone;
            this.Email = email;
            this.Mailer = mailer;
            this.Note = note;
        }
    }
}
