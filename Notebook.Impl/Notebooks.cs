using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Notebook.Interfaces;
using System.Net.Sockets;

namespace Notebook.Impl
{
    public static class Notebooks
    {
        public static INotebookLocal CreateLocalNotebook()
        {
            return new NotebookImpl();
        }
    }
}
