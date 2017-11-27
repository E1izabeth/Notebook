using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notebook.Interfaces
{
    public interface ISearchCriteriaVisitor
    {
        void VisitByName(ByNameSearchCriteria sc);
        void VisitBySurname(BySurnameSearchCriteria sc);
        void VisitByPhone(ByPhoneSearchCriteria sc);
        void VisitByEmail(ByEmailSearchCriteria sc);
    }

    public abstract class SearchCriteria
    {
        public string Text { get; private set; }

        public SearchCriteria(string text)
        {
            this.Text = text;
        }

        public void Apply(ISearchCriteriaVisitor visitor)
        {
            this.ApplyImpl(visitor);
        }

        protected abstract void ApplyImpl(ISearchCriteriaVisitor visitor);
    }

    public class ByNameSearchCriteria : SearchCriteria
    {
        public ByNameSearchCriteria(string text)
            : base(text) { }

        protected override void ApplyImpl(ISearchCriteriaVisitor visitor)
        {
            visitor.VisitByName(this);
        }
    }

    public class BySurnameSearchCriteria : SearchCriteria
    {
        public BySurnameSearchCriteria(string text)
            : base(text) { }

        protected override void ApplyImpl(ISearchCriteriaVisitor visitor)
        {
            visitor.VisitBySurname(this);
        }
    }

    public class ByPhoneSearchCriteria : SearchCriteria
    {
        public ByPhoneSearchCriteria(string text)
            : base(text) { }

        protected override void ApplyImpl(ISearchCriteriaVisitor visitor)
        {
            visitor.VisitByPhone(this);
        }
    }

    public class ByEmailSearchCriteria : SearchCriteria
    {
        public ByEmailSearchCriteria(string text)
            : base(text) { }

        protected override void ApplyImpl(ISearchCriteriaVisitor visitor)
        {
            visitor.VisitByEmail(this);
        }
    }

    public class SearchSpec
    {
        public ReadOnlyCollection<SearchCriteria> Conditions { get; private set; }

        public SearchSpec(params SearchCriteria[] conds)
        {
            this.Conditions = new ReadOnlyCollection<SearchCriteria>(conds);
        }
    }
}
