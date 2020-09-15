using System;
using Sinav.Data.Context;

namespace Sinav.Data.Models
{
    public class UserAnswer
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public Question Question { get; set; }

        public int QuestionOptionId { get; set; }
        public bool IsTrue { get; set; }
        public DateTime AnswerDate { get; set; }  = DateTime.Now;
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }
    }
}