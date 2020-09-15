using System.ComponentModel.DataAnnotations;

namespace Sinav.Web.DTOs
{
    public class NewAnnouncementDTO
    {
        [Required]
        public string Title { get; set; }
        [Required]

        public string Content { get; set; }

        public int? OrganizationId { get; set; }
    }
}