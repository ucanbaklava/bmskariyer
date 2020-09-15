namespace Sinav.Business.DTOs
{
    public class DenemeSonuc
    {
        public string[] Right { get; set; }
        public string[] User { get; set; }
        public int QuestionCount { get; set; }
        public int CorrectAnswerCount { get; set; }
        public int EmptyAnswerCount { get; set; }
    }
}