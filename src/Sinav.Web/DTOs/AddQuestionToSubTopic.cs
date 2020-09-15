using System.ComponentModel.DataAnnotations;

namespace Sinav.Web.DTOs
{
    public class AddQuestionToSubTopic
    {
        public int QuestionId { get; set; }
        [Required(ErrorMessage = "Konu Seçimi Zorunludur.")]
        public int SubTopicId { get; set; }
        [Required(ErrorMessage = "Soru İçeriği Boş Olamaz.")]

        public string Content { get; set; }
        [Required(ErrorMessage = "Tüm Şıkları Giriniz.")]

        public string a1 { get; set; }
        [Required(ErrorMessage = "Tüm Şıkları Giriniz.")]

        public string a2 { get; set; }
        [Required(ErrorMessage = "Tüm Şıkları Giriniz.")]

        public string a3 { get; set; }
        [Required(ErrorMessage = "Tüm Şıkları Giriniz.")]

        public string a4 { get; set; }
        [Required(ErrorMessage = "Tüm Şıkları Giriniz.")]

        public string a5 { get; set; }
        
        [Required(ErrorMessage = "Cevabı Seçiniz.")]

        public string Answer { get; set; }
        public string Anonym { get; set; }
        public string Explanation { get; set; }
        public string Cikmissoru { get; set; }

    }
}