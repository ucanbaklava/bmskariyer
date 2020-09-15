using System.Collections.Generic;
using Sinav.Data.Models;

namespace Sinav.Business.DTOs
{
    public class GetQuestionDTO
    {
        public int QuestionId { get; set; }
        public string SubTopic { get; set; }
        public string QuestionContent { get; set; }
        public string Sender { get; set; }
        public bool CikmisSoru { get; set; }
        public string SenderName { get; set; }
        public string Explanation { get; set; }
        public string Avatar { get; set; }
        public bool Anonym { get; set; }
        public int SubTopicId { get; set; }
        public string UserId { get; set; }
        public ICollection<Options> Options { get; set; } = new List<Options>();
    }
}