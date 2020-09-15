using System.Text;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Sinav.Business.Services.SubjectServices;

namespace Sinav.Web.TagHelpers
{

    [HtmlTargetElement("common-subjects")]
    public class CommonSubjectsTagHelper : TagHelper
    {

        private readonly ISubjectService _subjectService;

        public CommonSubjectsTagHelper(ISubjectService subjectService)
        {
            _subjectService = subjectService;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var subjects = _subjectService.GetCommonSubjects();
            StringBuilder sb = new StringBuilder();
            sb.Append(@"<div class='card mb-4'>
            <div class='card-header' style='background-color: #377aa3;color: #fff;font-weight: bold;letter-spacing: 0.07em;'>Alan Dışı Konular</div>
            <div class='card-body'>");
            foreach (var subject in subjects)
            {
                sb.Append($@"


                <div class='pb-1 mb-3'>
                <div class='badge badge-outline-info float-right'>Soru Sayısı: {subject.QuestionCount}</div>
                <a href='/detay/{subject.Slug}'>{subject.Name}</a><br>
                </div>");
            }

            sb.Append("</div></div>");
            output.Content.SetHtmlContent(sb.ToString());
            base.Process(context, output);
        }
    }
}
