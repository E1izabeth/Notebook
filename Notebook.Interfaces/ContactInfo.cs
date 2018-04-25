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
        

        public static bool ValidNumber(string number)
        {
            foreach (char letter in number)
            {
                if ((letter > '9' || letter < '0') && letter != '+' && letter != '(' && letter != ')' && letter != '-' && letter != ' ')
                {
                    return false;
                }
            }
            return true;
        }

        public static bool ValidDate(string date)
        {
            short count = 0;
            foreach (char letter in date)
            {
                if ((letter > '9' || letter < '0') && letter != '-')
                {
                    return false;
                }
                ++count;
            }
            if ((count != 10 || date[4] != '-' || date[7] != '-') && date != "")
                return false;

            return true;
        }

        public static bool ValidName(string name)
        {
            foreach (char letter in name)
                if (letter > 'z' || letter < 'A')
                {
                    return false;
                }
            return true;
        }

        public static bool ValidEmail(string email)
        {
            if ((email.Contains("@") && (email.Contains(".ru") || email.Contains(".com"))) || email == "")
            {
                return true;
            }
            else
                return false;
        }

    }
}
