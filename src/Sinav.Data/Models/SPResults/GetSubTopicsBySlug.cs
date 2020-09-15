namespace Sinav.Data.Models.SPResults
{
    public class GetSubTopicsBySlug
    {
        public string Name { get; set; }
        public string DocumentPath { get; set; }
        public string Slug { get; set; }
        public int QuestionCount { get; set; }
        public int SolvedQuestionCount { get; set; }
    }
}