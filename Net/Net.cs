using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace Net
{
    public class Net
    {
        public const ushort Port = 12345;

        static void Main(string[] args)
        {
            var srv = new Server.Server();
        }
    }
}
