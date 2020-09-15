using System;

namespace Sinav.Business.DTOs
{
    public class PreviousAnswerDto
    {
        public string AnswerDate { get; set; }
        public string Answer { get; set; }
        public int AnswerId { get; set; }
        public bool IsTrue { get; set; }
    }
}