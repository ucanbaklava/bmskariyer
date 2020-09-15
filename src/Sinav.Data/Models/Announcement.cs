using System;
using Sinav.Data.Context;

namespace Sinav.Data.Models
{
    public class Announcement
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime Date { get; set; }
        public string Title { get; set; }

        public string Slug { get; set; }
        public Organization Organization { get; set; }
        public int? OrganizationId { get; set; } = null;

        public bool IsDeleted { get; set; } = false;


    }
    
    
}