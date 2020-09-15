using Microsoft.AspNetCore.Http;

namespace Sinav.Web.DTOs
{
    public class NewSubjectDTO
    {
        public string Name { get; set; }
        public int? OrganizationId { get; set; }
        public IFormFile SubjectFile { get; set; }
    }
}