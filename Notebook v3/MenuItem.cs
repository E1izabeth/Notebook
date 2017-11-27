using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notebook_v3
{
    class MenuItem : IMenuItem
    {
        readonly List<MenuItem> _children = new List<MenuItem>();

        public IReadOnlyList<IMenuItem> Children { get; private set; }

        public string Text { get; private set; }
        public event Action OnSelected = delegate { };

        public MenuItem(string text)
        {
            this.Text = text;
            this.Children = new ReadOnlyCollection<MenuItem>(_children);
        }

        public MenuItem(string text, Action onSelected)
            : this(text)
        {
            this.OnSelected += onSelected;
        }
        
        public void Add(MenuItem child)
        {
            _children.Add(child);
        }

        void IMenuItem.InvokeHandler()
        {
            this.OnSelected();
        }

        #region IEnumerable<MenuItem> implementation

        public IEnumerator<IMenuItem> GetEnumerator()
        {
            return _children.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion
    }
}
