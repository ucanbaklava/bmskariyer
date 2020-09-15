using Microsoft.AspNetCore.Http;

namespace Sinav.Web.DTOs
{
    public class AddDocToSubjectDTO
    {
        public int SubjectId { get; set; }
        public IFormFile SubjectFile { get; set; }
    }
}