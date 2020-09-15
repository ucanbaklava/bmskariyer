using Microsoft.AspNetCore.Http;

namespace Sinav.Web.DTOs
{
    public class NewSubTopicDTO
    {
        public string Name { get; set; }
        public int SubjectId { get; set; }
        public IFormFile SubTopicFile { get; set; }
    }
}