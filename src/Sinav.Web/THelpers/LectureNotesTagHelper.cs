using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Sinav.Business.Services.SubTopicServices;
using Sinav.Data.Models;

namespace Sinav.Web.THelpers
{
    [HtmlTargetElement("lecture-notes", Attributes = "slug")]
    public class LectureNotesTagHelper : TagHelper
    {
        public string Slug { get; set; }
        private readonly ISubTopicService _subTopicService;

        public LectureNotesTagHelper(ISubTopicService subTopicService)
        {
            _subTopicService = subTopicService;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var lectureNotes = _subTopicService.GetLectureNotesBySubTopic(Slug);
            if (lectureNotes != null)
            {

            
                var sb = new StringBuilder();
                sb.Append(
                    @"<div class='card mb-4'>
                    <div class='card-header' style='background-color: crimson;color: #fff;font-weight: bold;letter-spacing: 0.07em;'>Ders Notları</div>
                <div class='card-body'>");
                sb.Append(@"<ul id='lecture-notes'>");
                var a = lectureNotes.GroupBy(x => x.SubTopic.Name);
                foreach (var note in lectureNotes.GroupBy(x => x.SubTopic.Name))
                {
                    sb.Append($@"<li><span class='caret'>{note.Key}</span>");
                    var ac = note.Select(x => x).ToList();
                    sb.Append(CreateChildNode(note.Select(x => x).ToList()));
                }
                sb.Append(@"</ul");
                sb.Append("</div></div>");

                output.Content.SetHtmlContent(sb.ToString());

                base.Process(context, output);
            }
        }
        static string CreateChildNode(List<LectureNote> lectureNotes)
        {
            var sb = new StringBuilder();
            sb.Append(@"<ul class='nested'>");
            
            foreach (var note in lectureNotes)
            {
                sb.Append($@"<li><a style='font-weight: 800;color: deepskyblue;' target='_blank' href='{note.Path}'>{note.Name + ' '} ({note.Extension}) </a></li>");
            }

            sb.Append(@"</ul>");
            return sb.ToString();
        }
    }

}