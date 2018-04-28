using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using Notebook.Interfaces;
using System.Diagnostics;
using System.Xml;
using System.Xml.Linq;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace Client
{
    public class Client : INotebookLocal
    {
        private string ServiceUrl { get; }
        private string Namespace { get; }
        private XmlNamespaceManager NamespaceManager { get; }

        public Client(int port)
        {
            this.Namespace = @"http://schemas.xml/";
            this.NamespaceManager = new XmlNamespaceManager(new NameTable());
            this.NamespaceManager.AddNamespace("x", this.Namespace);
            this.ServiceUrl = $@"http://127.0.0.1:{port}/";
        }

        public static INotebook SvcClient(int port)
        {
            var clientChannel = new TcpChannel();
            ChannelServices.RegisterChannel(clientChannel, false);

            return (INotebook)Activator.GetObject(typeof(INotebook), "tcp://127.0.0.1:9090/RemoteObject.rem");
        }

        public static INotebook WCFclient()
        {
            ChannelFactory<INotebook> factory = new ChannelFactory<INotebook>(new WebHttpBinding() {
            }, new EndpointAddress(@"http://127.0.0.1:9090/Notebook/"));
            factory.Endpoint.Behaviors.Add(new WebHttpBehavior() {
            });
            INotebook proxy = factory.CreateChannel();
            return proxy;
        }

        private void Send(XElement elem)
        {
            try
            {
                var xDoc = new XDocument(
                    new XDeclaration("1.0", "utf8", ""),
                    new XElement(
                            elem
                        )
                );

                var str = xDoc.ToString();



                //this.Socket.Send(Encoding.UTF8.GetBytes(str));

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
                //}

            }
            catch (SocketException ex)
            {
                Debug.Print(ex.ToString());
            }
        }

        private int RecieveCount()
        {
            var data = new byte[1];
            var builder = new StringBuilder();
            //int bytes = 0;
            //do
            //{
            //    bytes = this.Socket.Receive(data, data.Length, 0);
            //    builder.Append(Encoding.UTF8.GetString(data, 0, bytes));
            //}
            //while (this.Socket.Available > 0 && !builder.ToString().Contains("</Envelope>"));
            //XmlDocument xDoc = new XmlDocument();
            //XmlReaderSettings settings = new XmlReaderSettings();
            //using (NetworkStream stream = new NetworkStream(this.Socket))
            //using (XmlReader reader = XmlReader.Create(stream, settings))
            //{
            //    xDoc.Load(reader);
            //}

            return ParseXMlCount(builder.ToString());
        }

        private List<IContactInfo> RecieveContacts()
        {
            var data = new byte[1];
            var builder = new StringBuilder();
            //int bytes = 0;
            //do
            //{
            //    bytes = this.Socket.Receive(data, data.Length, 0);
            //    builder.Append(Encoding.UTF8.
            //         GetString(data, 0, bytes));
            //}
            //while (this.Socket.Available > 0 && !builder.ToString().Contains("</Envelope>"));
            //XmlDocument xDoc = new XmlDocument();
            //XmlReaderSettings settings = new XmlReaderSettings();
            //using (NetworkStream stream = new NetworkStream(this.Socket))
            //using (var reader = new StreamReader(stream))
            //{
            //    xDoc.Load(reader);
            //}

            return ParseXMlContacts(builder.ToString());
        }

        private int ParseXMlCount(string xmlString)
        {
            var obj = -1;

            XmlDocument xDoc = new XmlDocument();
            xDoc.LoadXml(xmlString);
            XmlElement xRoot = xDoc.DocumentElement;
            XmlNode node = xRoot.SelectSingleNode("//x:answer", this.NamespaceManager);
            if (node.FirstChild.Name == "count")
            {
                obj = Int32.Parse(node.FirstChild.InnerText);
            }
            return obj;
        }

        private List<IContactInfo> ParseXMlContacts(string xmlString)
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.LoadXml(xmlString);
            XmlElement xRoot = xDoc.DocumentElement;
            XmlNode node = xRoot.SelectSingleNode("//x:answer", this.NamespaceManager);

            List<IContactInfo> obj = new List<IContactInfo>();

            if (node.FirstChild.Name == "contacts")
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
                    obj.Add(cnt);
                }
            }
            return obj;
        }

        public int Count()
        {
            Send(new XElement(XName.Get("request", this.Namespace), "count"));
            return RecieveCount();
        }

        public IEnumerable<IContactInfo> GetContacts()
        {
            Send(new XElement(XName.Get("request", this.Namespace), "allcontacts"));
            return RecieveContacts().ToList();
        }

        public IEnumerable<IContactInfo> GetContacts(SearchSpec spec)
        {
            Send(new XElement(XName.Get("request", this.Namespace),
                new XElement(XName.Get("search", this.Namespace),
                    new XElement(XName.Get(spec.Conditions.First().GetType().ToString(), this.Namespace), spec.Conditions.First().Text),
                    spec.Conditions.Count == 1 ? null : new XElement(XName.Get(spec.Conditions.First().GetType().ToString(), this.Namespace), spec.Conditions.First().Text)
                    )
                )
            );

            return RecieveContacts().ToList();
        }

        public void NewElement(IContactInfo contact)
        {
            Send(new XElement(XName.Get("request", this.Namespace),
                new XElement(XName.Get("save", this.Namespace),
                    new XElement(XName.Get("contact", this.Namespace),
                        new XElement(XName.Get("n", this.Namespace), contact.FirstName),
                        new XElement(XName.Get("fn", this.Namespace), contact.LastName),
                        new XElement(XName.Get("nick", this.Namespace), contact.Nickname),
                        new XElement(XName.Get("bday", this.Namespace), contact.Birthday),
                        new XElement(XName.Get("tel", this.Namespace), contact.Phone),
                        new XElement(XName.Get("email", this.Namespace), contact.Email),
                        new XElement(XName.Get("mailer", this.Namespace), contact.Mailer),
                        new XElement(XName.Get("note", this.Namespace), contact.Note)
                        )
                    )
                )
           );
        }
    }
}