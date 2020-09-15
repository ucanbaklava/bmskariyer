using System.Text;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Sinav.Business.Services.PdfTestService;

namespace Sinav.Web.TagHelpers
{
  [HtmlTargetElement("subtopic-quiz",Attributes = "slug")]
    public class PdfQuizTagHelper: TagHelper
    {
        public string Slug { get; set; }
        private readonly IPdfTestService _pdfTestService;

        public PdfQuizTagHelper(IPdfTestService pdfTestService)
        {
            _pdfTestService = pdfTestService;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var tests = _pdfTestService.GetTestBySubTopic(Slug);
            var sb = new StringBuilder();

            sb.Append(@"<div class= 'card mb-4 '>
                  <h6 class= 'card-header with-elements '>
                    <span class= 'card-header-title '>Testler</span>
                    <div class= 'card-header-elements ml-auto '>
                      <button data-toggle='collapse' href='#pdftests' role='button' aria-expanded='false' class= 'btn btn-xs btn-outline-primary '>
                      </button>
                    </div>
                  </h6>
                  <ul class= 'list-group list-group-flush '>");
            foreach (var test in tests)
            {
              sb.Append($@"<li class= 'list-group-item ' id='pdftests'>
                      <div class= 'media align-items-center '>
                        <i class='fas fa-pen'></i>
                        <div class= 'media-body px-2 '>
                          <a href= '/test/{test.Slug}' class= 'text-body '>{test.Name}</a>
                        </div>
                        <a href= 'javascript:void(0) ' class= 'd-block text-light text-large font-weight-light '>Soru Sayısı: {test.QuestionCount}</a>
                      </div>
                    </li>");
            }

            sb.Append(@"</ul></div>");
            output.Content.SetHtmlContent(sb.ToString());

            base.Process(context, output);
        }
    }
}