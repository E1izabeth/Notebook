using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace VCard
{
    public class VCardEntry
    {
        string _groupName;
        List<ContentLine> _linesCollection;
    }

    class ContentLine
    {
        string _nameLine;
        List<Params> _paramsCollection;
        string _valueLine;
    }

    class Params
    {
        string _paramName;
        string[] _paramValues;
    }

    public class VCardFile
    {
        List<VCardEntry> _vCard_collection = new List<VCardEntry>();


        public static void WriteTo(TextWriter writer)
        {
            
        }

        public static VCardFile ReadFrom(TextReader reader)
        {
            reader.ReadLine();
            VCardFile _vCardFile = new VCardFile();
            
            try
            {
                while (reader.Peek() != -1)
                {
                    VCardEntry contact = new VCardEntry();
                    string line = reader.ReadLine();
                    while (!line.Contains("END"))
                    {
                        line = reader.ReadLine();
                        
                    }
                    //main body from begin to end read in vCardEntry
                    _vCardFile._vCard_collection.Add(contact);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }
            
            return _vCardFile;
        }



        private object[] ParseLine(string line)
        {
            object[] res = new object[2];
            Match m = Regex.Match(line, "^(?<tag>[A-Za-z]*)[:;]");
            if (m.Success)
            {
                string tag = m.Groups["tag"].Value;
                line = line.Replace(string.Format("{0}:", tag), "");
                switch (tag.ToLower())
                {
                    case "version":
                        {
                            Match ma = Regex.Match(line, @"(?<version>[\d.]+)");
                            res[0] = "version";
                            res[1] = float.Parse(ma.Groups["version"].Value);
                            return res;
                        }
                    case "fn":
                        {
                            return null;
                        }
                    case "n":
                        {
                            Match ma = Regex.Match(line, @":(?<familyname>[\w\W\s]*);(?<givenname>[\w\W\s]*);(?<additionalnames>[\w\W\s]*);(?<honorificprefixes>[\w\W\s]*);(?<honorificsuffixes>[\w\W\s]*)");
                            res[0] = "n";
                            res[1] = ma.Groups["familyname"].Value + ma.Groups["givenname"].Value + ma.Groups["additionalnames"].Value + ma.Groups["honorificprefixes"].Value + ma.Groups["honorificsuffixes"].Value;
                            return res;
                        }
                    case "nickname":
                        {
                            Match ma = Regex.Match(line, "(?<nickname>[A-Za-z,]*)$");
                            res[0] = "nickname";
                            res[1] = ma.Groups["nickname"].Value;
                            return res;
                        }
                    case "bday":
                        {
                            Match ma = Regex.Match(line, ":(?<date>[0-9]{4}-[0-9]{2}-[0-9]{2}");
                            res[0] = "bday";
                            Match mb = Regex.Match(ma.Groups["date"].Value, "(?<year>[0-9]+)[-/.](?<month>[0-9]+)[-/.](?<day>[0-9]+)");
                            res[1] = mb.Groups["year"].Value + mb.Groups["month"].Value + mb.Groups["day"].Value;
                            return res;
                        }
                    case "tel":
                        {
                            Match ma = Regex.Match(line, @";(?<types>[A-Za-z,]*):(?<number>[-+\d]+)");
                            res[0] = "tel";
                            res[1] = ma.Groups["number"].Value;
                            return res;
                        }
                    case "email":
                        {
                            Match ma = Regex.Match(line, @"(?<types>[A-Za-z,]*):(?<email>[\w\W]+)");
                            res[0] = "email";
                            res[1] = ma.Groups["email"].Value;
                            return res;
                        }
                    case "mailer":
                        {
                            Match ma = Regex.Match(line, @":(?<mailer>[\w\d\s\W]+)$");
                            res[0] = "mailer";
                            res[1] = ma.Groups["mailer"].Value;
                            return res;
                        }
                    case "note":
                        {
                            Match ma = Regex.Match(line, @":(?<note>[.]+)$");
                            res[0] = "note";
                            res[1] = ma.Groups["mailer"].Value;
                            return res;
                        }
                    default:
                        {
                            return null;
                        }
                }
            }
            return null;
        }

        /*public string ToVCard()
        {
            var builder = new StringBuilder();
            builder.AppendLine("BEGIN:VCARD");
            builder.AppendLine("VERSION:3.0");
            builder.AppendLine("N:" + this.LastName + ";" + this.FirstName);
            builder.AppendLine("FN:" + this.FirstName + " " + this.LastName);
            builder.AppendLine("NICKNAME:" + this.Nickname);
            builder.AppendLine("BDAY:" + this.Birthday);
            builder.AppendLine("TEL;HOME;VOICE:" + this.Phone);
            builder.AppendLine("EMAIL;PREF;INTERNET:" + this.Email);
            builder.AppendLine("MAILER:" + this.Mailer);
            builder.AppendLine("NOTE:" + this.Note);
            builder.AppendLine("END:VCARD");
            return builder.ToString();
        }

        public void TryLoadVCard(string[] DataSource, Contact _newContact)
        {

        }*/
    }
}
