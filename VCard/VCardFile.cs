using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace VCard
{
    public class Names
    {
        public const string VERSION = "VERSION";
        public const string N = "N";
        public const string FN = "FN";
        public const string NICKNAME = "NICKNAME";
        public const string BDAY = "BDAY";
        public const string TEL = "TEL;HOME;VOICE";
        public const string EMAIL = "EMAIL;PREF;INTERNET";
        public const string MAILER = "MAILER";
        public const string NOTE = "NOTE";
    }

    public class VCardEntry
    {
        public string GroupName { get; private set; }
        public List<ContentLine> Contents { get; private set; }

        public VCardEntry(string groupName, params ContentLine[] contents)
        {
            this.GroupName = groupName;
            this.Contents = new List<ContentLine>(contents.ToArray());
        }
    }

    public class ContentLine
    {
        public string Name { get; private set; }
        public ReadOnlyCollection<Param> Params { get; private set; }
        public string Value { get; private set; }

        public ContentLine(string name, string value, params Param[] items)
        {
            this.Name = name;
            this.Params = new ReadOnlyCollection<Param>(items.ToArray());
            this.Value = value;
        }
    }

    public class Param
    {
        public string Name { get; private set; }
        public ReadOnlyCollection<string> Values { get; private set; }

        public Param(string name, params string[] values)
        {
            this.Name = name;
            this.Values = new ReadOnlyCollection<string>(values.ToArray());
        }
    }

    public class VCardFile
    {
        // @"^(?<group>[\w\-]+\.)?(?<name>[\w\-]+)(\;((?<param>[\w\-]+)(\=(?<pvalue>(\"[^\"]*\")|([^\"\;\:\,]*))(\,(?<pvalue>(\"[^\"]*\")|([^\"\;\:\,]*)))?)?))*:(?<value>.*)";

        enum ContentLinePartName
        {
            group, name, param, pvalue, value
        }

        const string _contentLinePattern = "^(?<group>[\\w\\-]+\\.)?(?<name>[\\w\\-]+)(\\;((?<param>[\\w\\-]+)(\\=(?<pvalue>(\\\"[^\\\"]*\\\")|([^\\\"\\;\\:\\,]*))(\\,(?<pvalue>(\\\"[^\\\"]*\\\")|([^\\\"\\;\\:\\,]*)))?)?))*:(?<value>.*)$";

        static Regex _contentLineRegex = new Regex(_contentLinePattern);

        public List<VCardEntry> Entries { get; private set; }

        public VCardFile(params VCardEntry[] entries)
        {
            this.Entries = new List<VCardEntry>(entries.ToArray());
        }

        public void WriteTo(TextWriter writer)
        {
            var builder = new StringBuilder();

            foreach (VCardEntry TmpContact in this.Entries)
            {
                builder.AppendLine("BEGIN:VCARD");
                var GroupName = TmpContact.GroupName;
                foreach (ContentLine line in TmpContact.Contents)
                {
                    builder.Append(GroupName/* + "."*/ + line.Name); //.
                    foreach (Param param in line.Params)
                    {
                        builder.Append(";" + param.Name + "=");
                        foreach (string value in param.Values)
                        {
                            builder.Append(value + ",");
                        }
                    }
                    builder.AppendLine(":" + line.Value);
                }
                builder.AppendLine("END:VCARD");
            }

            builder.ToString();
            writer.Write(builder);
        }

        public static VCardFile ReadFrom(TextReader reader)
        {
            VCardFile file = new VCardFile();

            string line;
            while (reader.Peek() != -1)
            {
                VCardEntry entry = new VCardEntry("");
                line = reader.ReadLine();
                line = reader.ReadLine();
                while (line.ToLower() != "end:vcard")
                {
                    ContentLine res = ParseLine(string.Empty, line);
                    entry.Contents.Add(res);
                    line = reader.ReadLine();
                }
                file.Entries.Add(entry);
            }
            return file;
        }

        public static ContentLine ParseLine(string gname, string line)
        {
            var match = _contentLineRegex.Match(line);
            // Match m = Regex.Match(line, @"^(?<groupname>[.*\.])(?<tag>[A-Za-z]*)[:;]");//[.]*\.      [././]*
            if (match.Success)
            {
                var allTokens = Enum.GetValues(typeof(ContentLinePartName)).OfType<ContentLinePartName>().SelectMany(
                    pn => match.Groups[pn.ToString()].Captures.OfType<Capture>()
                               .Select(c => new { pn, c.Value, c.Index })
                ).OrderBy(p => p.Index).Select(c => new KeyValuePair<ContentLinePartName, string>(c.pn, c.Value.Trim())).ToArray();

                var tokens = new Queue<KeyValuePair<ContentLinePartName, string>>(allTokens);
                var token = tokens.Dequeue();
                if (token.Key == ContentLinePartName.group)
                {
                    if (token.Value == gname)
                        throw new NotImplementedException("Invalid group");

                    token = tokens.Dequeue();
                }

                if (token.Key != ContentLinePartName.name)
                    throw new NotImplementedException("Invalid name");

                var lineName = token.Value;
                token = tokens.Dequeue();

                var lineParams = new List<Param>();
                while(token.Key == ContentLinePartName.param)
                {
                    var paramName = token.Value;
                    var paramValues = new List<string>();
                    token = tokens.Dequeue();
                    while (token.Key == ContentLinePartName.pvalue)
                    {
                        paramValues.Add(token.Value);
                        token = tokens.Dequeue();
                    }

                    lineParams.Add(new Param(paramName, paramValues.ToArray()));
                }
                if (token.Key != ContentLinePartName.value)
                    throw new NotImplementedException("Invalid value");
                var lineValue = token.Value;

                ContentLine contentLine = new ContentLine(
                    lineName,
                    lineValue,
                    lineParams.ToArray()
                );
                return contentLine;
            }
            return null;
        }
    }
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

                /*string tag = m.Groups["tag"].Value;
                line = line.Replace(string.Format("{0}:", tag), "");
                Match ma = Regex.Match(line, @"(?<groupname>[.*\.])(?<content>[.]*)$");
                string res = ma.Groups["content"].Value;
                return res;
                 switch (tag.ToLower())
                 {
                     case "version":
                         {
                             Match ma = Regex.Match(line, @"(?<groupname>[.*\.])(?<version>[\d].[\d])$");
                             res[0] = "version";
                             res[1] = ma.Groups["version"].Value;
                             return res;
                         }
                     case "fn":
                         {
                             Match ma = Regex.Match(line, @"(?<groupname>[.*\.])(?<familyname>[\w\W\s]*) (?<givenname>[\w\W\s]*)$");
                             res[0] = "fn";
                             res[1] = ma.Groups["givenname"].Value;
                             return res;
                         }
                     case "n":
                         {
                             Match ma = Regex.Match(line, @"(?<groupname>[.*\.])(?<familyname>[\w\W\s]*);(?<givenname>[\w\W\s]*)$");
                             res[0] = "n";
                             res[1] = ma.Groups["familyname"].Value;
                             return res;
                         }
                     case "nickname":
                         {
                             Match ma = Regex.Match(line, @"(?<groupname>[.*\.])(?<nickname>[A-Za-z,]*)$");
                             res[0] = "nickname";
                             res[1] = ma.Groups["nickname"].Value;
                             return res;
                         }
                     case "bday":
                         {
                             Match ma = Regex.Match(line, "(?<date>[0-9]{4}-[0-9]{2}-[0-9]{2})$");
                             res[0] = "bday";
                             Match mb = Regex.Match(ma.Groups["date"].Value, "(?<year>[0-9]+)[-/.](?<month>[0-9]+)[-/.](?<day>[0-9]+)$");
                             res[1] = mb.Groups["year"].Value + "-" + mb.Groups["month"].Value + "-" + mb.Groups["day"].Value;
                             return res;
                         }
                     case "tel":
                         {
                             Match ma = Regex.Match(line, @";(?<types>[A-Za-z,]*):(?<number>[-+\d]+)$");
                             res[0] = "tel";
                             res[1] = ma.Groups["number"].Value;
                             return res;
                         }
                     case "email":
                         {
                             Match ma = Regex.Match(line, @"(?<types>[A-Za-z,]*):(?<email>[\w\W]+)$");
                             res[0] = "email";
                             res[1] = ma.Groups["email"].Value;
                             return res;
                         }
                     case "mailer":
                         {
                             Match ma = Regex.Match(line, @"(?<groupname>[.*\.])(?<mailer>[\w\d\s\W]+)$");
                             res[0] = "mailer";
                             res[1] = ma.Groups["mailer"].Value;
                             return res;
                         }
                     case "note":
                         {
                             Match ma = Regex.Match(line, @"(?<groupname>[.*\.])(?<note>[\w\W\s]*)$");
                             res[0] = "note";
                             res[1] = ma.Groups["note"].Value;
                             return res;
                         }
                     default:
                         {
                             return null;
                         }
                 }*/