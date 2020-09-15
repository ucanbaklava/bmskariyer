namespace Sinav.Business.DTOs
{
    public class ListQuestionReports
    {
        public int Id { get; set; }
        public string Feedback { get; set; }
        public string UserName { get; set; }
        public int QuestionId { get; set; }
        public string Avatar { get; set; }
        public string Subject { get; set; }
        public string SubTopic { get; set; }

    }
}