using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.UI;

using Rant.Vocabulary;

namespace Rave.DicDoc
{
	public static class PageGenerator
    {
        public static string GenerateTablePage(RantDictionaryTable table, string filename)
        {
            int entryCount = table.GetEntries().Count();

            // Get all the classes
            var tableClasses = new HashSet<string>();
            foreach (var entry in table.GetEntries())
            {
                foreach (var entryClass in entry.GetClasses())
                {
                    tableClasses.Add(entryClass);
                }
            }

            var text = new StringWriter();

            using (var writer = new HtmlTextWriter(text))
            {
                writer.WriteLine("<!DOCTYPE html>");

                writer.RenderBeginTag(HtmlTextWriterTag.Html);

                // Header
                writer.RenderBeginTag(HtmlTextWriterTag.Head);

                // Title
                writer.RenderBeginTag(HtmlTextWriterTag.Title);
                writer.WriteEncodedText(table.Name);
                writer.RenderEndTag();

                // Stylesheet
                writer.AddAttribute(HtmlTextWriterAttribute.Rel, "stylesheet");
                writer.AddAttribute(HtmlTextWriterAttribute.Type, "text/css");
                writer.AddAttribute(HtmlTextWriterAttribute.Href, "dicdoc.css");
                writer.RenderBeginTag(HtmlTextWriterTag.Link);
                writer.RenderEndTag();

                writer.RenderEndTag(); // </head>

                writer.RenderBeginTag(HtmlTextWriterTag.Body);

                // Header
                writer.RenderBeginTag(HtmlTextWriterTag.H1);
                writer.WriteEncodedText("<" + table.Name + ">");
                writer.RenderEndTag();

                // Description
                writer.RenderBeginTag(HtmlTextWriterTag.P);
                writer.WriteEncodedText("The ");
                writer.RenderBeginTag(HtmlTextWriterTag.B);
                writer.WriteEncodedText(table.Name);
                writer.RenderEndTag(); // </b>
                writer.WriteEncodedText(" table ("
                    + Path.GetFileName(filename)
                    + ") contains "
                    + entryCount + (entryCount == 1 ? " entry" : " entries ")
                    + " and " + tableClasses.Count + (tableClasses.Count == 1 ? " class" : " classes")
                    + ".");
                writer.RenderEndTag(); // </p>

                // Subtypes
                if (table.Subtypes.Length == 1)
                {
                    writer.WriteEncodedText("It has one subtype: ");
                    writer.RenderBeginTag(HtmlTextWriterTag.B);
                    writer.WriteEncodedText(table.Subtypes[0]);
                    writer.RenderEndTag();
                    writer.WriteEncodedText(".");
                }
                else
                {
                    writer.WriteEncodedText("It has " + table.Subtypes.Length + (table.Subtypes.Length == 1 ? " subtype" : " subtypes")
                        + ": ");
                    for(int i = 0; i < table.Subtypes.Length; i++)
                    {
                        if (i == table.Subtypes.Length - 1)
                            writer.WriteEncodedText(table.Subtypes.Length == 2 ? " and " :  "and ");

                        writer.RenderBeginTag(HtmlTextWriterTag.B);
                        writer.WriteEncodedText(table.Subtypes[i]);
                        writer.RenderEndTag();

                        if (i < table.Subtypes.Length - 1 && table.Subtypes.Length > 2)
                            writer.WriteEncodedText(", ");
                    }
                    writer.WriteEncodedText(".");
                }

                // Separator
                writer.RenderBeginTag(HtmlTextWriterTag.Hr);
                writer.RenderEndTag();

                // "View All" link
                writer.AddAttribute(HtmlTextWriterAttribute.Href, "entries/" + table.Name + ".html");
                writer.RenderBeginTag(HtmlTextWriterTag.A);

                writer.WriteEncodedText("Browse All Entries");

                writer.RenderEndTag(); // </a>

                // Class list
                writer.RenderBeginTag(HtmlTextWriterTag.H2);
                writer.WriteEncodedText("Classes");
                writer.RenderEndTag(); // </h2>

                writer.RenderBeginTag(HtmlTextWriterTag.Ul);

                foreach (var tableClass in tableClasses)
                {
                    writer.RenderBeginTag(HtmlTextWriterTag.Li);
                    
                    writer.AddAttribute(HtmlTextWriterAttribute.Href, "entries/" + table.Name + "-" + tableClass + ".html");
                    writer.RenderBeginTag(HtmlTextWriterTag.A);
                    writer.WriteEncodedText(tableClass);
                    writer.RenderEndTag(); // </a>

                    writer.RenderEndTag(); // </li>
                }

                writer.RenderEndTag(); // </ul>

                writer.RenderEndTag(); // </body>

                writer.RenderEndTag(); // </html>
            }

            return text.ToString();
        }

        public static string GenerateTableClassPage(RantDictionaryTable table, string tableClass)
        {
            bool all = String.IsNullOrEmpty(tableClass);

            var entries = all ? table.GetEntries() : table.GetEntries().Where(e => e.ContainsClass(tableClass));

            var text = new StringWriter();

            using (var writer = new HtmlTextWriter(text))
            {
                writer.WriteLine("<!DOCTYPE html>");

                writer.RenderBeginTag(HtmlTextWriterTag.Html);

                // Header
                writer.RenderBeginTag(HtmlTextWriterTag.Head);

                // Title
                writer.RenderBeginTag(HtmlTextWriterTag.Title);
                writer.WriteEncodedText((all ? table.Name : table.Name + ": " + tableClass) + " entries");
                writer.RenderEndTag();

                // Stylesheet
                writer.AddAttribute(HtmlTextWriterAttribute.Rel, "stylesheet");
                writer.AddAttribute(HtmlTextWriterAttribute.Type, "text/css");
                writer.AddAttribute(HtmlTextWriterAttribute.Href, "../dicdoc.css");
                writer.RenderBeginTag(HtmlTextWriterTag.Link);
                writer.RenderEndTag();

                writer.RenderEndTag(); // </head>

                // Body
                writer.RenderBeginTag(HtmlTextWriterTag.Body);

                // Heading
                writer.RenderBeginTag(HtmlTextWriterTag.H1);
                writer.WriteEncodedText("<" + table.Name + (all ? "" : "-" + tableClass) + ">");
                writer.RenderEndTag();

                // Entry list
                foreach (var e in entries)
                {
                    writer.AddAttribute(HtmlTextWriterAttribute.Class, "entry");
                    writer.RenderBeginTag(HtmlTextWriterTag.Div);

                    // Terms
                    writer.AddAttribute(HtmlTextWriterAttribute.Class, "termset");
                    writer.RenderBeginTag(HtmlTextWriterTag.Div);

                    for (int i = 0; i < e.Terms.Length; i++)
                    {
                        writer.AddAttribute(HtmlTextWriterAttribute.Class, "term");
                        writer.RenderBeginTag(HtmlTextWriterTag.Span);
                        writer.WriteEncodedText(e.Terms[i].Value);

                        if (e.Terms[i].PronunciationParts.Length > 0)
                        {
                            writer.AddAttribute(HtmlTextWriterAttribute.Class, "terminfo");
                            writer.RenderBeginTag(HtmlTextWriterTag.Span);
                            writer.RenderBeginTag(HtmlTextWriterTag.I);
                            writer.WriteEncodedText(" [" + e.Terms[i].Pronunciation + "]");
                            writer.RenderEndTag(); // </i>
                            writer.RenderEndTag(); // </span>
                        }

                        // Subtype
                        writer.AddAttribute(HtmlTextWriterAttribute.Class, "subtype");
                        writer.RenderBeginTag(HtmlTextWriterTag.Span);
                        writer.WriteEncodedText(i < table.Subtypes.Length ? table.Subtypes[i] : "???");
                        writer.RenderEndTag();

                        writer.RenderEndTag();
                    }

                    writer.RenderEndTag();

                    // Notes
                    var notes = new List<string>();
                    var otherClasses = e.GetClasses().Where(cl => cl != tableClass);

                    if (e.Terms.All(t => t.PronunciationParts.Length > 0))
                        notes.Add("Full pronunciation");
                    else if (e.Terms.Any(t => t.PronunciationParts.Length > 0))
                        notes.Add("Partial pronunciation");

                    if (e.Weight != 1) notes.Add("Weight: " + e.Weight);

                    if (all && e.GetClasses().Any())
                    {
                        notes.Add("Classes: " + String.Join(", ", e.GetClasses()));
                    }
                    else
                    {
                        if (otherClasses.Any()) notes.Add("Other classes: " + String.Join(", ", otherClasses));
                    }
                    

                    if (e.ContainsClass("nsfw")) notes.Add("NSFW");

                    GenerateUL(writer, notes);

                    writer.RenderEndTag();
                }

                writer.RenderEndTag(); // </body>

                writer.RenderEndTag(); // </html>
            }

            return text.ToString();
        }

        private static void GenerateUL(HtmlTextWriter writer, IEnumerable<string> items)
        {
            if (items == null || !items.Any()) return;

            writer.RenderBeginTag(HtmlTextWriterTag.Ul);

            foreach (var item in items)
            {
                writer.RenderBeginTag(HtmlTextWriterTag.Li);
                writer.WriteEncodedText(item);
                writer.RenderEndTag();
            }

            writer.RenderEndTag();
        }
    }
}