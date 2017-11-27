using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Notebook.Interfaces;

namespace Notebook.Impl
{
    class Contact : IContactInfo
    {
        public string Name { get; private set; }
        public string Surname { get; private set; }
        public string Phone { get; private set; }
        public string Email { get; private set; }

        public Contact(string name, string surname, string phone, string email)
        {
            this.Name = name;
            this.Surname = surname;
            this.Phone = phone;
            this.Email = email;
        }
    }

    class NotebookImpl : INotebook
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
                _result = _result.Concat(_initial.Where(c => c.Name.Contains(sc.Text)));
            }

            void ISearchCriteriaVisitor.VisitByPhone(ByPhoneSearchCriteria sc)
            {
                _result = _result.Concat(_initial.Where(c => c.Phone.Contains(sc.Text)));
            }

            void ISearchCriteriaVisitor.VisitBySurname(BySurnameSearchCriteria sc)
            {
                _result = _result.Concat(_initial.Where(c => c.Surname.Contains(sc.Text)));
            }

            #endregion

            public void Run(SearchSpec spec)
            {
                foreach (var item in spec.Conditions)
                    item.Apply(this);
            }

            public IEnumerable<IContactInfo> GetSubset()
            {
                return _result.ToArray();
            }
        }

        readonly List<Contact> _list;

        public NotebookImpl()
        {
            _list = new List<Contact>();
        }

        public void NewElement(IContactInfo nc)
        {
            _list.Add(new Contact(nc.Name, nc.Surname, nc.Phone, nc.Email));
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
    }
}
