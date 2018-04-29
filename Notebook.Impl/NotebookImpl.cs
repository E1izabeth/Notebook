using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Notebook.Interfaces;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Runtime.Serialization;

namespace Notebook.Impl
{
    class Contact : IContactInfo
    {
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Nickname { get; private set; }
        public string Birthday { get; private set; }
        public string Phone { get; private set; }
        public string Email { get; private set; }
        public string Mailer { get; private set; }
        public string Note { get; private set; }

        public Contact(string FirstName, string LastName, string Nickname, string Birthday,
                                    string Phone, string Email, string Mailer, string Note)
        {
            this.FirstName = FirstName;
            this.LastName = LastName;
            this.Nickname = Nickname;
            this.Birthday = Birthday;
            this.Phone = Phone;
            this.Email = Email;
            this.Mailer = Mailer;
            this.Note = Note;
        }
    }

    //[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class NotebookImpl : MarshalByRefObject, INotebookLocal
    {
        class QueryExecutor : ISearchCriteriaVisitor
        {
            IEnumerable<Contact> _initial;
            IEnumerable<Contact> _result;

            public QueryExecutor(IEnumerable<Contact> subset)
            {
                _initial = subset;
                _result = new Contact[0];
            }

            #region ISearchCriteriaVisitor impl

            void ISearchCriteriaVisitor.VisitByEmail(ByEmailSearchCriteria sc)
            {
                _result = _result.Concat(_initial.Where(c => c.Email.Contains(sc.Text)));
            }

            void ISearchCriteriaVisitor.VisitByName(ByNameSearchCriteria sc)
            {
                _result = _result.Concat(_initial.Where(c => c.FirstName.Contains(sc.Text)));
            }

            void ISearchCriteriaVisitor.VisitByPhone(ByPhoneSearchCriteria sc)
            {
                _result = _result.Concat(_initial.Where(c => c.Phone.Contains(sc.Text)));
            }

            void ISearchCriteriaVisitor.VisitBySurname(BySurnameSearchCriteria sc)
            {
                _result = _result.Concat(_initial.Where(c => c.LastName.Contains(sc.Text)));
            }

            #endregion

            public void Run(SearchSpec spec)
            {
                foreach (var item in spec.Conditions.Where(cond => cond != null))
                    item.Apply(this);
            }

            public IEnumerable<Contact> GetSubset()
            {
                var arr = _result.ToArray();
                var result = arr.Distinct();
                return result;
            }
        }

        public DateTime Ping()
        {
            return DateTime.Now;
        }

        readonly List<Contact> _list;
        readonly RWLock _lock = new RWLock();
        
        public NotebookImpl()
        {
            _list = new List<Contact>();
        }

        public void NewElement(IContactInfo nc)
        {
            using (_lock.Write())
            {
                _list.Add(new Contact(nc.FirstName, nc.LastName, nc.Nickname, nc.Birthday, nc.Phone, nc.Email, nc.Mailer, nc.Note));
            }
        }

        public IEnumerable<IContactInfo> GetContacts()
        {
            using (_lock.Read())
            {
                return ConvertListOfContactToListOfContactInfo(_list);
            }
        }
        
        public IEnumerable<IContactInfo> GetContacts(SearchSpec spec)
        {
            using (_lock.Read())
            {
                var executor = new QueryExecutor(_list);

                executor.Run(spec);

                return ConvertListOfContactToListOfContactInfo(executor.GetSubset().ToList());
            }
        }

        public int Count()
        {
            using (_lock.Read())
            {
                return _list.Count;
            }
        }

        private IEnumerable<IContactInfo> ConvertListOfContactToListOfContactInfo(List<Contact> list)
        {
            var newList = new List<ContactInfo>(list.Count);
            foreach (var item in list)
            {
                var cnt = new ContactInfo {
                    FirstName = item.FirstName,
                    LastName = item.LastName,
                    Birthday = item.Birthday,
                    Nickname = item.Nickname,
                    Email = item.Email,
                    Phone = item.Phone,
                    Mailer = item.Mailer,
                    Note = item.Note
                };
                newList.Add(cnt);
            }
            return newList;
        }

    }
}
