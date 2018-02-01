using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Notebook.Interfaces;

namespace Server
{
    public class Server
    {
        public INotebook Book { get; private set; }
        private Socket Socket { get; }
        private string Namespace { get; }
        private XmlNamespaceManager NamespaceManager { get; }

        public Server()
        {
            this.Namespace = @"http://schemas.xmlsoap.org/soap/envelope/";
            this.NamespaceManager = new XmlNamespaceManager(new NameTable());
            this.NamespaceManager.AddNamespace("x", this.Namespace);
            this.Book = new Notebook.Impl.NotebookImpl();
            Recieve();
        }

        private void Recieve()
        {
            try
            {

                var listener = new HttpListener();
                listener.Prefixes.Add("http://127.0.0.1:12345/");
                listener.Start();
                do
                {
                    var context = listener.GetContext();
                    HttpListenerRequest request = context.Request;
                    HttpListenerResponse response = context.Response;
                    var url = context.Request.RawUrl;
                    var stream = request.InputStream;
                    string[] pairs;
                    using (var reader = new StreamReader(stream, Encoding.UTF8))
                        pairs = reader.ReadToEnd().Split('&');
                    

                    var resultBytes = Encoding.UTF8.GetBytes(ParseQuery(url, pairs));
                    context.Response.ContentEncoding = Encoding.UTF8;
                    context.Response.ContentLength64 = resultBytes.Length;
                    context.Response.Close(resultBytes, false);
                } while (listener.IsListening);
                listener.Stop();
            }
            catch (SocketException ex)
            {
                Debug.Print(ex.ToString());
            }
            
        }



        private string ParseQuery(string url, string[] pairs)
        {
            string resultText = "";

            switch (url)
            {
                case "/count":
                    {
                        resultText = "count:" + this.Book.Count().ToString();
                        break;
                    }
                case "/allcontacts":
                    {
                        var list = this.Book.GetContacts();
                        resultText = PerformContactsToString(list);
                        break;
                    }
                case "/search":
                    {
                        List<SearchCriteria> list = new List<SearchCriteria>();
                        for (int i = 0; i < pairs.Length; i++)
                        {
                            string crit = pairs[i].Remove(pairs[i].IndexOf("="));
                            string search = pairs[i].Substring(pairs[i].IndexOf("=") + 1);
                            if (crit.Contains("ByNameSearchCriteria"))
                                list.Add(new ByNameSearchCriteria(search));
                            else if (crit.Contains("BySurnameSearchCriteria"))
                                list.Add(new BySurnameSearchCriteria(search));
                            else if (crit.Contains("ByPhoneSearchCriteria"))
                                list.Add(new ByPhoneSearchCriteria(search));
                            else if (crit.Contains("ByEmailSearchCriteria"))
                                list.Add(new ByEmailSearchCriteria(search));
                            else
                                throw new NotSupportedException();
                        }
                        resultText = PerformContactsToString(this.Book.GetContacts(new SearchSpec(list.ToArray())));
                        break;
                    }
                case "/save":
                    {
                        ParseContacts(pairs);
                        resultText = "OK";
                        break;
                    }
                default:
                    throw new NotSupportedException();
            }

            return resultText;
        }

        private string PerformContactsToString(IEnumerable<IContactInfo> enumerable)
        {
            string resultText = "";
            foreach (var item in enumerable)
            {
                resultText += "n:" + item.FirstName + "\n";
                resultText += "fn:" + item.LastName + "\n";
                resultText += "nick:" + item.Nickname + "\n";
                resultText += "bday:" + item.Birthday + "\n";
                resultText += "tel:" + item.Phone + "\n";
                resultText += "email:" + item.Email + "\n";
                resultText += "mailer:" + item.Mailer + "\n";
                resultText += "note:" + item.Note + "\n";
            }
            return resultText;
        }

        private void ParseContacts(string[] pairs)
        {
            for (int i = 0; i < pairs.Length; )
            {
                var cnt = new ContactInfo();
                do
                {
                    var subpair = pairs[i].Split('=');
                    switch (subpair[0])
                    {
                        case "n":
                            {
                                cnt.FirstName = subpair[1];
                                break;
                            }
                        case "fn":
                            {
                                cnt.LastName = subpair[1];
                                break;
                            }
                        case "nick":
                            {
                                cnt.Nickname = subpair[1];
                                break;
                            }
                        case "bday":
                            {
                                cnt.Birthday = subpair[1];
                                break;
                            }
                        case "tel":
                            {
                                cnt.Phone = subpair[1];
                                break;
                            }
                        case "email":
                            {
                                cnt.Email = subpair[1].Replace("%40", "@");
                                break;
                            }
                        case "mailer":
                            {
                                cnt.Mailer = subpair[1];
                                break;
                            }
                        case "note":
                            {
                                cnt.Note = subpair[1];
                                break;
                            }
                        default:
                            break;
                    }
                    ++i;
                }
                while (i % 8!= 0);
                this.Book.NewElement(cnt);
            }
        }

    }
}
