using Notebook.Impl;
using Notebook.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.ServiceModel;
using System.ServiceModel.Description;


namespace Net
{
    public class Net
    {
        public const ushort Port = 9090;

        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.FirstChanceException += (sender, ea) => {
                System.Diagnostics.Debug.Print(ea.Exception.ToString());
            };

            //var svcImpl = new Connector();
            //var srv = new Server.Server(svcImpl);

            var impl = new Connector();
            using (ServiceHost host = new ServiceHost(impl, new Uri(@"http://127.0.0.1:9090/Notebook")))
            {
                var ep = host.AddServiceEndpoint(typeof(INotebook), new WebHttpBinding(), "");
                ep.EndpointBehaviors.Add(new WebHttpBehavior());
                ServiceMetadataBehavior behavior = host.Description.Behaviors.Find<ServiceMetadataBehavior>();
                if (behavior == null)
                {
                    behavior = new ServiceMetadataBehavior { HttpGetEnabled = true };
                    host.Description.Behaviors.Add(behavior);
                }
                host.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexHttpBinding(), "mex");
                host.Opening += new EventHandler(Host_Opening);
                host.Opened += new EventHandler(Host_Opened);
                host.Open();

                    Console.ReadLine();
            }

        }
            static void Host_Opened(object sender, EventArgs e)
            {
                Console.WriteLine("Service is ready!\nPress <ENTER> key to terminate service....\n");
            }

            static void Host_Opening(object sender, EventArgs e)
            {
                Console.WriteLine("Opening service.....");
            }



            //try
            //{
            //    var serverChannel = new TcpChannel(Port);
            //    ChannelServices.RegisterChannel(serverChannel, false);
            //    RemotingConfiguration.RegisterWellKnownServiceType(typeof(NotebookImpl), "RemoteObject.rem", WellKnownObjectMode.Singleton);


            //    Console.WriteLine("Press Enter to stop the server");
            //    while (Console.ReadKey().Key != ConsoleKey.Enter) { }
            //}
            //catch (Exception ex)
            //{
            //    Debug.Print(ex.ToString());
            //}


            //srv.DoWork();

    }
}
