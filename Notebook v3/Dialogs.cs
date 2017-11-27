using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Notebook.Interfaces;

namespace Notebook_v3
{
    class Dialogs
    {
        public IContactInfo AddNew()
        {
            ContactInfo note = new ContactInfo();
            Console.Clear();
            Console.WriteLine("New contact:");
            if ((note.Name = ReadProp("Name: ", "Invalid name!", ValidName)) == null)
                return AddNew();
            if ((note.Surname = ReadProp("Surname: ", "Invalid surname!", ValidName)) == null)
                return AddNew();
            if ((note.Phone = ReadProp("Phone: ", "Invalid phone!", ValidNumber)) == null)
                return AddNew();
            if ((note.Email = ReadProp("E-mail: ", "Invalid email!", ValidEmail)) == null)
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
                Console.WriteLine("Please, press any button to try again...");
                Console.ReadKey();
                value = null;
            }

            return value;
        }

        public string AskRequest()
        {
            Console.Write("Request: ");
            string answer = Console.ReadLine();
            Console.WriteLine("\nSearching...");
            return answer;
        }

        public void PrintSelection(List<IContactInfo> book)
        {
            Console.Clear();
            Console.WriteLine("Results(" + book.Count + ")");
            this.PrintCollectionImpl(book);
            Console.WriteLine("Press any button to continue...");
            Console.ReadKey();
        }

        private void PrintCollectionImpl(List<IContactInfo> contacts)
        {
            int i = 0;
            foreach (IContactInfo tmp in contacts)
            {
                i += 1;
                Console.Write("#" + i + "\tName: " + tmp.Name + "\n");
                Console.WriteLine("\tSurname: " + tmp.Surname);
                Console.WriteLine("\tPhone: " + tmp.Phone);
                Console.WriteLine("\tE-mail: " + tmp.Email);
            }
            if (contacts.Count == 0)
                Console.WriteLine("No contacts");
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
            if (email.Contains("@") && (email.Contains(".ru") || email.Contains(".com")))
            {
                return true;
            }
            else
                return false;
        }
    }
}
