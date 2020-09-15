using System.Collections;
using System.Collections.Generic;

namespace Sinav.Data.Models
{
    public class Subject
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public Document Document { get; set; }
        public int? DocumentId { get; set; }
        public Organization Organization { get; set; }

        public int? OrganizationId { get; set; }

        public string Slug { get; set; }
        public ICollection<SubTopic> SubTopics { get; set; } = new List<SubTopic>();
        public bool IsDeleted { get; set; } = false;


    }
}