using System.Collections.Generic;

namespace Sinav.Data.Models
{
    public class SubTopic
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Subject Subject { get; set; }
        public string Slug { get; set; }

        public int SubjectId { get; set; }
        public ICollection<Question> Questions = new List<Question>();
        public ICollection<LectureNote> LectureNotes { get; set; } = new List<LectureNote>();
        public bool IsDeleted { get; set; } = false;


    }
}