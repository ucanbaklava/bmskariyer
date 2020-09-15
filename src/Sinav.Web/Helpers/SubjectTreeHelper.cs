using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Sinav.Data.Models;

namespace Sinav.Web.Helpers
{
    public static class SubjectTreeHelper
    {
        public static string TreeviewHelper(List<Subject> items)
        {
            var builder = new TagBuilder("ul");
            foreach (var item in items)
            {

                builder.InnerHtml.AppendHtml(AddItem(item));
                if (item.SubTopics.Any())
                {
                    builder.InnerHtml.AppendHtml(CreateChildItem(item.SubTopics.ToList()));
                }

            }

            var htmlOutput = "";
            using (var writer = new StringWriter())
            {
                builder.WriteTo(writer, System.Text.Encodings.Web.HtmlEncoder.Default);
                htmlOutput = writer.ToString();
                // now use 'htmlOutput'
            }
            return htmlOutput;
        }

        public static TagBuilder CreateChildItem(List<SubTopic> subtopics)
        {
            var ul = new TagBuilder("ul");
            foreach (var subtopic in subtopics)
            {
                var li = new TagBuilder("li");
                var radio = new TagBuilder("input");
                li.AddCssClass("ls-none");
                radio.Attributes["type"] = "radio";
                radio.Attributes["name"] = "subtopicId";
                radio.Attributes["value"] = Convert.ToString(subtopic.Id);
                radio.AddCssClass("mr-1");
                li.InnerHtml.AppendHtml(radio);
                li.InnerHtml.Append(subtopic.Name);
                ul.InnerHtml.AppendHtml(li);
            }

            return ul;
        }

        public static TagBuilder AddItem(Subject subject)
        {

            var li = new TagBuilder("li");
            li.InnerHtml.Append(subject.Name);
            return li;
        }
    }
}