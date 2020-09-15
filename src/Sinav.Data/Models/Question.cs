using System;
using System.Collections.Generic;
using Sinav.Data.Context;

namespace Sinav.Data.Models
{
    public class Question
    {
        public int Id { get; set; }
        public string Text { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public ICollection<Options> Options { get; set; }= new List<Options>();
        public ICollection<UserAnswer> UserAnswers { get; set; } = new List<UserAnswer>();
        public SubTopic SubTopic { get; set; }
        public int SubTopicId { get; set; }

        public string Explanation { get; set; }
        public string AppuserId { get; set; }
        public AppUser? AppUser { get; set; }

        public bool IsDeleted { get; set; } = false;
        public bool Published { get; set; } = true;
        public DateTime? ApprovedAt { get; set; }
        public bool Anonym { get; set; } = false;
        public bool CikmisSoru { get; set; }
        

    }
}