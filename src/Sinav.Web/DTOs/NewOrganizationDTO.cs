using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace Sinav.Web.DTOs
{
    public class NewOrganizationDTO
    {
        public string Name { get; set; }
        public IFormFile Image { get; set; }
        public int Id { get; set; }
        public string WebRootPath { get; set; }
        public string Path { get; set; }
    }
}