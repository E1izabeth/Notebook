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
    public static class Names
    {
        public const string VERSION = "VERSION";
        public const string N = "N";
        public const string FN = "FN";
        public const string NICKNAME = "NICKNAME";
        public const string BDAY = "BDAY";
        public const string TEL = "TEL";//;HOME;VOICE";
        public const string EMAIL = "EMAIL";//;PREF;INTERNET";
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

    [Serializable]
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
                    builder.Append(GroupName/* + "."*/ + line.Name);
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