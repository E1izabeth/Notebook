using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Soap;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Notebook.Interfaces;

namespace Server
{
    public class Server
    {
        public INotebookLocal Book { get; private set; }
        private Socket Socket { get; }
        private string Namespace { get; }
        private XmlNamespaceManager NamespaceManager { get; }

        public Server(INotebookLocal svcImpl)
        {
            this.Book = svcImpl;
        }

        public Server()
        {
            this.Namespace = @"http://schemas.xml/";
            this.NamespaceManager = new XmlNamespaceManager(new NameTable());
            this.NamespaceManager.AddNamespace("x", this.Namespace);
            this.Book = new Notebook.Impl.NotebookImpl();
            var lsck = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            lsck.Bind(new IPEndPoint(IPAddress.Any, 12345));
            lsck.Listen(2);
            Console.WriteLine("Waiting for connection.. ");
            var sck = lsck.Accept();
            lsck.Close();
            Console.WriteLine("Connected");
            this.Socket = sck;
            DoWork(sck);
        }

        public void DoWork(Socket sck)
        {
            while (sck.Connected)
            {
                //var sth = new Thread(() => { });
                var rth = new Thread(() => Recieve());

                //sth.Start();
                rth.Start();

                //sth.Join();
                rth.Join();
            }
            Console.WriteLine("Disconnected");
        }

        private void Recieve()
        {
            //while (this.Socket.Connected)
            //{
            try
            {
                XmlDocument xDoc = new XmlDocument();
                var data = new byte[1];
                StringBuilder builder = new StringBuilder();
                int bytes = 0;
                int sumBytes = 0;
                do
                {
                    bytes = this.Socket.Receive(data, data.Length, 0);
                    builder.Append(Encoding.UTF8.GetString(data, 0, bytes));
                    sumBytes += bytes;
                    if (builder.ToString().Contains("</Envelope>"))
                    {
                        xDoc.LoadXml(builder.ToString());
                        ParseXMl(xDoc);
                    }
                }
                while (this.Socket.Available > 0);
                //xDoc.LoadXml(builder.ToString());
                //ParseXMl(xDoc);
                //XmlReaderSettings settings = new XmlReaderSettings();
                //using (NetworkStream stream = new NetworkStream(this.Socket))
                //using (XmlReader reader = XmlReader.Create(stream, settings))
                //{
                //    xDoc.Load(reader);
                //}

                //Thread.Sleep(1000);
                //this.Socket.Shutdown(SocketShutdown.Receive);
            }
            catch (SocketException ex)
            {
                Debug.Print(ex.ToString());
            }

        }

        private void Send(XElement elem)
        {
            try
            {
                XDocument xDoc = new XDocument(
                    new XDeclaration("1.0", "utf8", ""),
                    new XElement(
                        XName.Get("Envelope", this.Namespace),
                        new XElement(
                            XName.Get("Body", this.Namespace),
                            elem
                        )
                    )
                );
                var str = xDoc.ToString();
                this.Socket.Send(Encoding.UTF8.GetBytes(str));
                Console.WriteLine(@"{0}    Send {1} bytes", DateTime.Now, str.Length);
                //using (NetworkStream stream = new NetworkStream(this.Socket))
                //{
                //    XmlWriterSettings settings = new XmlWriterSettings {
                //        Encoding = Encoding.UTF8,
                //        OmitXmlDeclaration = true
                //    };
                //    using (XmlWriter writer = XmlWriter.Create(stream, settings))
                //    {
                //        xDoc.Save(writer);
                //    }
            }
            catch (SocketException ex)
            {
                Debug.Print(ex.ToString());
            }
        }

        private void ParseXMl(XmlDocument xDoc)
        {
            XmlElement xRoot = xDoc.DocumentElement;
            XmlNode node = xRoot.SelectSingleNode("//x:request", this.NamespaceManager);
            if (node.FirstChild.Value == "count")
            {
                Send(new XElement(
                        XName.Get("answer", this.Namespace),
                        new XElement(
                            XName.Get("count", this.Namespace),
                            this.Book.Count()
                            )
                         ));
            }
            else if (node.FirstChild.Value == "allcontacts")
            {
               Send(ContactListForSend(this.Book.GetContacts().ToList()));
            }
            else if (node.FirstChild.Name == "search")
            {
                List<SearchCriteria> list = new List<SearchCriteria>();
                foreach (XmlNode crit in node.FirstChild)
                {
                    switch (crit.Name)
                    {
                        case "Notebook.Interfaces.ByNameSearchCriteria":
                            {
                                list.Add(new ByNameSearchCriteria(crit.InnerText));
                                break;
                            }
                        case "Notebook.Interfaces.BySurameSearchCriteria":
                            {
                                list.Add(new BySurnameSearchCriteria(crit.InnerText));
                                break;
                            }
                        case "Notebook.Interfaces.ByPhoneSearchCriteria":
                            {
                                list.Add(new ByPhoneSearchCriteria(crit.InnerText));
                                break;
                            }
                        case "Notebook.Interfaces.ByEmailSearchCriteria":
                            {
                                list.Add(new ByEmailSearchCriteria(crit.InnerText));
                                break;
                            }
                        default:
                            break;
                    }
                }

                Send(ContactListForSend(this.Book.GetContacts(new SearchSpec(list.ToArray())).ToList()));
            }
            else if (node.FirstChild.Name == "save")
            {
                ParseXMlContacts(node, xDoc);
            }

        }


        private void ParseXMlContacts(XmlNode node, XmlDocument xDoc)
        {
            foreach (XmlNode contact in node.FirstChild)
            {
                var cnt = new ContactInfo();
                foreach (XmlNode info in contact)
                {
                    switch (info.Name)
                    {
                        case "n":
                            {
                                cnt.FirstName = info.InnerText;
                                break;
                            }
                        case "fn":
                            {
                                cnt.LastName = info.InnerText;
                                break;
                            }
                        case "nick":
                            {
                                cnt.Nickname = info.InnerText;
                                break;
                            }
                        case "bday":
                            {
                                cnt.Birthday = info.InnerText;
                                break;
                            }
                        case "tel":
                            {
                                cnt.Phone = info.InnerText;
                                break;
                            }
                        case "email":
                            {
                                cnt.Email = info.InnerText;
                                break;
                            }
                        case "mailer":
                            {
                                cnt.Mailer = info.InnerText;
                                break;
                            }
                        case "note":
                            {
                                cnt.Note = info.InnerText;
                                break;
                            }
                        default:
                            break;
                    }
                }
                this.Book.NewElement(cnt);
            }
        }

        private XElement ContactListForSend(List<IContactInfo> cnt)
        {
            return new XElement(XName.Get("answer", this.Namespace),
                new XElement(XName.Get("contacts", this.Namespace),
                    cnt.Select(t =>
                    new XElement(
                                    XName.Get("contact", this.Namespace),
                                    new XElement(XName.Get("n", this.Namespace), t.FirstName),
                                    new XElement(XName.Get("fn", this.Namespace), t.LastName),
                                    new XElement(XName.Get("nick", this.Namespace), t.Nickname),
                                    new XElement(XName.Get("bday", this.Namespace), t.Birthday),
                                    new XElement(XName.Get("tel", this.Namespace), t.Phone),
                                    new XElement(XName.Get("email", this.Namespace), t.Email),
                                    new XElement(XName.Get("mailer", this.Namespace), t.Mailer),
                                    new XElement(XName.Get("note", this.Namespace), t.Note)
                                )
                            )
                    )
                );
        }
    }
}
