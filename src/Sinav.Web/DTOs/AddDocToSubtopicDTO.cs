using Microsoft.AspNetCore.Http;

namespace Sinav.Web.DTOs
{
    public class AddDocToSubtopicDTO
    {
        public int SubTopicId { get; set; }
        public IFormFile SubjectFile { get; set; }
        public string Name { get; set; }
    }
}