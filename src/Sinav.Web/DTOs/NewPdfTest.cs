using Microsoft.AspNetCore.Http;

namespace Sinav.Web.DTOs
{
    public class NewPdfTest
    {
        public string Name { get; set; }
        public string Answers { get; set; }
        public IFormFile Pdf { get; set; }
        public int Time { get; set; }
        public int SubjectId { get; set; }
        public int SubTopicId { get; set; }
        public int? OrgId { get; set; }
        public string Overall { get; set; }
    }
}