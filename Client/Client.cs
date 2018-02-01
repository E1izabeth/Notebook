using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using Notebook.Interfaces;
using System.IO;
using System.Diagnostics;
using System.Xml;
using System.Xml.Linq;
using System.Net.Http;
using System.Net;
using System.Collections.Specialized;

namespace Client
{
    public class Client : INotebook
    {
        private string Url { get; }
        private XmlNamespaceManager NamespaceManager { get; }

        public Client(int port)
        {
            this.Url = String.Format(@"http://127.0.0.1:{0}/", port);
        }

        private string Agent(string type, NameValueCollection request)
        {
            try
            {
                using (var client = new WebClient())
                {
                    client.Encoding = Encoding.UTF8;
                    var ans = client.UploadValues(String.Format("{0}{1}", this.Url, type), request);
                    return Encoding.UTF8.GetString(ans);
                }

            }
            catch (SocketException ex)
            {
                Debug.Print(ex.ToString());
            }
            return null;
        }

        public int Count()
        {
            return RecievedCount(Agent("count", new NameValueCollection()));
        }

        private int RecievedCount(string answer)
        {
            var info = answer.Split(':');
            if (info[0] == "count")
                return Int32.Parse(info[1]);
            else
                return 0; //?
        }

        public IEnumerable<IContactInfo> GetContacts()
        {
            return RecieveContacts(Agent("allcontacts", new NameValueCollection())).ToList();
        }

        private IEnumerable<IContactInfo> RecieveContacts(string answer)
        {
            List<IContactInfo> contactsList = new List<IContactInfo>();
            string[] infos = answer.Split('\n');
            for (int i = 0; i < infos.Length - 1; )
            {
                var cnt = new ContactInfo();
                do
                {
                    var subpair = infos[i].Split(':');
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
                while (i % 8 != 0);
                contactsList.Add(cnt);
            }

            return contactsList;
        }

        public IEnumerable<IContactInfo> GetContacts(SearchSpec spec)
        {
            var pars = new NameValueCollection();
            pars.Add(spec.Conditions.First().GetType().ToString(), spec.Conditions.First().Text);
            if (spec.Conditions.Count > 1)
            {
                pars.Add(spec.Conditions.ElementAt(1).GetType().ToString(), spec.Conditions.ElementAt(1).Text);
            }

            return RecieveContacts(Agent("search", pars)).ToList();
        }

        public void NewElement(IContactInfo cnt)
        {
            var pars = new NameValueCollection {
                { "n", cnt.FirstName },
                { "fn", cnt.LastName },
                { "nick", cnt.Nickname },
                { "bday", cnt.Birthday },
                { "tel", cnt.Phone },
                { "email", cnt.Email },
                { "mailer", cnt.Mailer },
                { "note", cnt.Note }
            };
            Agent("save", pars);
        }
    }
}