using System;
using Sinav.Data.Context;

namespace Sinav.Data.Models
{
    public class PdfTestResult
    {
        public int Id { get; set; }
        public PDFTest PDFTest { get; set; }
        public int PDFTestId { get; set; }
        public AppUser AppUser { get; set; }
        public string AppuserId { get; set; }
        public int RightAnswerCount { get; set; }
        public int WrongAnswerCount { get; set; }
        public int EmptyAnswerCount { get; set; }
        public double Score { get; set; }
        public DateTimeOffset DateofSubmit { get; set; } = DateTimeOffset.Now;
        public string UserAnswer { get; set; }
        
    }
}