namespace Sinav.Business.DTOs
{
    public class ListAllSubjectsDto
    {
        public int Id { get; set; }    
        public string Name { get; set; }
        public string DocumentPath { get; set; }
        public string FileSize { get; set; }
        public int SubTopicCount { get; set; }
    }
}