namespace Sinav.Data.Models
{
    public class Options
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public bool isCorrect { get; set; }

        public Question Question { get; set; }
        public int QuestionId { get; set; }
    }
}