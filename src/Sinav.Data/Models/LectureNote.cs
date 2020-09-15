namespace Sinav.Data.Models
{
    public class LectureNote
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Size { get; set; }
        public string Extension { get; set; }
        public string Source { get; set; }
        public string Excerpt { get; set; }
        public string Path { get; set; }

        public SubTopic SubTopic { get; set; }
    }
}