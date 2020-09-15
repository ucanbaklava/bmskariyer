using System.Text;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Sinav.Business.Services.AnnouncementServices;

namespace Sinav.Web.TagHelpers
{
    [HtmlTargetElement("announcement-list",Attributes = "usid")]

    public class AnnouncementTagHelper: TagHelper
    {
        private readonly IAnnouncementService _announcementService;
        public string Usid { get; set; }


        public AnnouncementTagHelper(IAnnouncementService announcementService)
        {
            _announcementService = announcementService;
        }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            StringBuilder sb = new StringBuilder();
            sb.Append(
                @"<div class='card mb-4'>
                    <div class='card-header' style='background-color: crimson;color: #fff;font-weight: bold;letter-spacing: 0.07em;'>DUYURULAR</div>
                <div class='card-body'>");
            var logo = string.Empty;
            
            foreach (var ann in _announcementService.GetUserAnnouncements(1,10, "", Usid))
            {
                if (ann.OrganizationId != null)
                {
                    logo = ann.Organization.OrgImage;
                }
                else
                {
                    logo = "/assets/images/logo.png";
                }
                sb.Append(
                    $@"<div class='pb-1 mb-3'>
                    <img src='{logo}' width='40px;/'>
                    <a href='/duyuru/{ann.Slug}'>{ann.Title}</a>&nbsp
                    <small class='text-muted'>Tarih: {ann.Date}</small>
                    </div>");
            }

            sb.Append("</div></div>");
            output.Content.SetHtmlContent(sb.ToString());            
            base.Process(context, output);

            
        }
    }
}