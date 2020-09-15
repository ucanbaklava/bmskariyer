using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Sinav.Business.DTOs;
using Sinav.Business.Services.PdfTestService;

namespace Sinav.Web.TagHelpers
{
  [HtmlTargetElement("deneme",Attributes = "user-id")]
    public class DenemeTagHelper: TagHelper
    {
        public string UserId { get; set; }
        private readonly IPdfTestService _pdfTestService;

        public DenemeTagHelper(IPdfTestService pdfTestService)
        {
            _pdfTestService = pdfTestService;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var tests = _pdfTestService.GetPdfsByUserId(UserId);
            var sb = new StringBuilder();

            sb.Append(@"<div class= 'card mb-4 '>
                  <h6 class= 'card-header with-elements '>
                    <span class= 'card-header-title '>Genel Denemeler</span>
                    <div class= 'card-header-elements ml-auto '>
                      <button data-toggle='collapse' href='#overallpdftests' role='button' aria-expanded='false' class= 'btn btn-xs btn-outline-primary '>
                      </button>
                    </div>
                  </h6>
                  <ul class= 'list-group list-group-flush '>");
            foreach (var test in tests)
            {
              sb.Append($@"<li class= 'list-group-item ' id='overallpdftests'>
                      <div class= 'media align-items-center '>
                        <img src='{test.Org?.OrgImage ?? "/assets/images/logo.png"}' width=40px;/>
                        <div class= 'media-body px-2 '>
                          <a href= '/test/{test.Slug}' class= 'text-body '>{test.Name}</a>
                        </div>
                          <a data-toggle='tooltip' data-placement='top' data-state='primary' data-original-title='Puan Sıralaması' href='/siralama/{test.Slug}'<i class='fas fa-poll' style='padding-right: 10px;width:;font-size: 1.7em;color: crimson;'></i></a>
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