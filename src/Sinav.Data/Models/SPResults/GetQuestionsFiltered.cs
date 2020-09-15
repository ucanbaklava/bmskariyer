namespace Sinav.Data.Models.SPResults
{
    public class GetQuestionsFiltered
    {
        public string UserId { get; set; }
        public string Avatar { get; set; }
        public bool Anonym { get; set; }
        public string QuestionContent { get; set; }
        public int QuestionId { get; set; }
        public string UserName { get; set; }
        public string SubTopic { get; set; }
        public string Subject { get; set; }
        public int Total { get; set; }
    }
}