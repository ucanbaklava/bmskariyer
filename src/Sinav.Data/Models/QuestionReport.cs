using Sinav.Data.Context;

namespace Sinav.Data.Models
{
    public class QuestionReport
    {
        public int Id { get; set; }
        public Question Question { get; set; }
        public int QuestionId { get; set; }
        public string Feedback { get; set; }
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }

        public bool IsDeleted { get; set; } = false;
    }
}