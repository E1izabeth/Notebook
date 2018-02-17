using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Notebook.Interfaces;

namespace Notebook
{
    class Dialogs
    {
        public ContactInfo AddNew()
        {
            ContactInfo note = new ContactInfo();
            Console.WriteLine(Notebook_v3.Properties.text.New_contact);
            if ((note.FirstName = ReadProp(Notebook_v3.Properties.text.First_name + ": ", Notebook_v3.Properties.text.Invalid_first_name, ValidName)) == null)
                return AddNew();
            if ((note.LastName = ReadProp(Notebook_v3.Properties.text.Last_name + ": ", Notebook_v3.Properties.text.Invalid_last_name, ValidName)) == null)
                return AddNew();
            if ((note.Nickname = ReadProp(Notebook_v3.Properties.text.Nickname + ": ", Notebook_v3.Properties.text.Invalid_nickname, ValidName)) == null)
                return AddNew();
            if ((note.Birthday = ReadProp(Notebook_v3.Properties.text.Birthday + "(YYYY-MM-DD): ", Notebook_v3.Properties.text.Invalid_birthday, ValidDate)) == null)
                return AddNew();
            if ((note.Phone = ReadProp(Notebook_v3.Properties.text.Phone + ": ", Notebook_v3.Properties.text.Invalid_phone, ValidNumber)) == null)
                return AddNew();
            if ((note.Email = ReadProp(Notebook_v3.Properties.text.Email+ ": ", Notebook_v3.Properties.text.Invalid_email, ValidEmail)) == null)
                return AddNew();
            if ((note.Mailer = ReadProp(Notebook_v3.Properties.text.Mailer + ": ", "", ValidAlways)) == null)
                return AddNew();
            if ((note.Note = ReadProp(Notebook_v3.Properties.text.Note + ": ", "", ValidAlways)) == null)
                return AddNew();
            return note;
        }

        private string ReadProp(string message, string errMsg, Func<string, bool> validateAction)
        {
            Console.Write(message);
            string value = Console.ReadLine();
            if (!validateAction(value))
            {
                Console.WriteLine(errMsg);
                Console.WriteLine(Notebook_v3.Properties.text.ButtonAgain);
                Console.ReadKey();
                value = null;
            }

            return value;
        }

        public string AskRequest()
        {
            Console.Write(Notebook_v3.Properties.text.Request + ": ");
            string answer = Console.ReadLine();
            Console.WriteLine(Notebook_v3.Properties.text.Searching);
            return answer;
        }

        public string LoadPath()
        {
            Console.Write(Notebook_v3.Properties.text.InputPath + ": ");
            string _path = Console.ReadLine();
            return _path;
        }

        public void PrintSelection(List<IContactInfo> book)
        {
            Console.WriteLine(Notebook_v3.Properties.text.Results + "(" + book.Count + ")");
            this.PrintCollectionImpl(book);
            Console.WriteLine(Notebook_v3.Properties.text.ButtonContinue);
            Console.ReadKey();
        }

        public void PrintSelection(IContactInfo[] book)
        {
            Console.WriteLine(Notebook_v3.Properties.text.Results + "(" + book.Count() + ")");
            this.PrintCollectionImpl(book.ToList<IContactInfo>());
            Console.WriteLine(Notebook_v3.Properties.text.ButtonContinue);
            Console.ReadKey();
        }

        private void PrintCollectionImpl(List<IContactInfo> contacts)
        {
            int i = 0;
            foreach (IContactInfo tmp in contacts)
            {
                i += 1;
                Console.Write("#" + i + "\t" + Notebook_v3.Properties.text.First_name + ": " + tmp.FirstName + "\n");
                Console.WriteLine("\t" + Notebook_v3.Properties.text.Last_name + ": " + tmp.LastName);
                Console.WriteLine("\t" + Notebook_v3.Properties.text.Nickname + ": " + tmp.Nickname);
                Console.WriteLine("\t" + Notebook_v3.Properties.text.Birthday + ": " + tmp.Birthday);
                Console.WriteLine("\t" + Notebook_v3.Properties.text.Phone + ": " + tmp.Phone);
                Console.WriteLine("\t" + Notebook_v3.Properties.text.Email + ": " + tmp.Email);
                Console.WriteLine("\t" + Notebook_v3.Properties.text.Mailer + ": " + tmp.Mailer);
                Console.WriteLine("\t" + Notebook_v3.Properties.text.Note + ": " + tmp.Note);
            }
            if (contacts.Count == 0)
                Console.WriteLine(Notebook_v3.Properties.text.No_contacts);
        }

        public static bool ValidAlways(string str)
        {
            return true;
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
            if (count != 10 || date[4]!='-' || date[7]!='-')
                return false;
            else
                return true;
        }

        public static bool ValidName(string name)
        {
            foreach (char letter in name)
                if (letter > Notebook_v3.Properties.text.z.ToCharArray().First() || letter < Notebook_v3.Properties.text.A.ToCharArray().First())
                {
                    return false;
                }
            return true;
        }

        public static bool ValidEmail(string email)
        {
            if (/*!email.Contains("%40") &&*/ email.Contains("@") && (email.Contains(".ru") || email.Contains(".com")))
            {
                return true;
            }
            else
                return false;
        }
    }
}
