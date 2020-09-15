using System.Collections.Generic;

namespace Sinav.Data.Models
{
    public class PDFTest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; } = false;

        public bool Overall { get; set; }
        public Organization Org { get; set; }
        public int? OrganizationId { get; set; } = null;
        public Subject Subject { get; set; }
        public int? SubjectId { get; set; }
        public SubTopic SubTopic { get; set; }
        public int? SubTopicId { get; set; }
        public int Time { get; set; }
        public string Answers { get; set; } 
        public string PdfPath { get; set; }
        public int QuestionCount { get; set; }
        public string Slug { get; set; }
    }
}