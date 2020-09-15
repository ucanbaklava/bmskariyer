namespace Sinav.Web.DTOs
{
    public class QuestionFilterDTO
    {
        public bool PreviouslyAsked { get; set; }
        public bool NotSolved { get; set; }
        public bool SolvedFalse { get; set; }
        public int QuestionCount { get; set; }
        public bool RandomOrder { get; set; }
        public string Slug { get; set; }
    }
}