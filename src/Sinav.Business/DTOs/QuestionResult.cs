namespace Sinav.Business.DTOs
{
    public class QuestionResult
    {
        public bool IsCorrect { get; set; }
        public string UserAnswer { get; set; }
        public string CorrectAnswer { get; set; }
    }
}