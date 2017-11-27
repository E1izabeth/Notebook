using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Notebook.Interfaces;

namespace Notebook.Impl
{
    public static class Notebooks
    {
        public static INotebook CreateLocalNotebook()
        {
            return new NotebookImpl();
        }
    }
}
