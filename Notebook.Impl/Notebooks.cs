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
        public static INotebook CreateLocalNotebook()
        {
            return new NotebookImpl();
        }

        public static INotebook CreateRemoteNotebook(int port)
        {
            return new Client.Client(port);
        }
    }
}
