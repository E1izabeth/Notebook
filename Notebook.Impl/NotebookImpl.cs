using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Notebook.Interfaces;
using System.IO;


namespace Notebook.Impl
{
    [Serializable]
    class Contact: IContactInfo
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

    public class NotebookImpl : INotebook
    {
        class QueryExecutor : ISearchCriteriaVisitor
        {
            IEnumerable<IContactInfo> _initial;
            IEnumerable<IContactInfo> _result;

            public QueryExecutor(IEnumerable<IContactInfo> subset)
            {
                _initial = subset;
                _result = new IContactInfo[0];
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
                foreach (var item in spec.Conditions)
                    item.Apply(this);
            }

            public IEnumerable<IContactInfo> GetSubset()
            {
                var arr = _result.ToArray();
                var result = arr.Distinct();
                return result;
            }
        }

        readonly List<Contact> _list;
       
        //TODO: don't forget!
        public NotebookImpl()
        {
            _list = new List<Contact>();
            _list.Add(new Contact("qqwer", "qdcvgfds", "qsda", "22.33.4421", "12345", "q@m.ru", "q", "q"));
            _list.Add(new Contact("frwer", "css", "hre", "21.32.4426", "12543", "q@m.ru", "q", "q"));
        }

        public void NewElement(IContactInfo nc)
        {
            _list.Add(new Contact(nc.FirstName, nc.LastName,nc.Nickname, nc.Birthday, nc.Phone, nc.Email, nc.Mailer, nc.Note));
        }
        
        public IEnumerable<IContactInfo> GetContacts()
        {
            return _list.ToArray();
        }

        public IEnumerable<IContactInfo> GetContacts(SearchSpec spec)
        {
            var executor = new QueryExecutor(_list);

            executor.Run(spec);

            return executor.GetSubset();
        }

        public int Count()
        {
            return _list.Count;
        }

    }
}
