using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Sinav.Web.DTOs
{
    public class UserRegisterDTO
    {
        [Required(ErrorMessage = "Email kısmı boş bırakılamaz.")]
        [Display(Name = "Email")]
        [StringLength( 50, ErrorMessage = "Mail adresi 100 karakterden uzun olamaz." )]
        public string Email { get; set; }
        
        [Required(ErrorMessage = "Ad kısmı boş bırakılamaz.")]
        [Display(Name = "Ad")]
        [StringLength( 100, ErrorMessage = "Ad 100 karakterden uzun olamaz." )]

        public string FirstName { get; set; }
        
        [Required(ErrorMessage = "Soyad kısmı boş bırakılamaz.")]
        [Display(Name = "Soyad")]
        [StringLength( 100, ErrorMessage = "Soyad 100 karakterden uzun olamaz." )]

        public string LastName { get; set; }
        
        [Display(Name = "Resim")]
        [DataType(DataType.Upload)]
        public IFormFile Avatar { get; set; }
        
        [Required(ErrorMessage = "Şifre kısmı boş bırakılamaz.")]
        [DataType(DataType.Password)]
        [Display(Name = "Şifre")]
        public string Password { get; set; }
        
        [Required(ErrorMessage = "Şifre Tekrar kısmı boş bırakılamaz.")]
        [Display(Name = "Parola (Tekrar)")]
        [Compare("Password", ErrorMessage = "Şifreler aynı değil.")]
        public string PasswordConfirm { get; set; }
        
        [Required(ErrorMessage = "Kurum boş bırakılamaz.")]
        public int OrganizationId { get; set; }
        [Required(ErrorMessage = "Kadro boş bırakılamaz.")]
        
        public int StaffId { get; set; }
        
        [Required(ErrorMessage = "Cinsiyet boş bırakılamaz.")]
        public int Gender { get; set; }
        
        [Required(ErrorMessage = "Telefon numarası boş bırakılamaz.")]
        [RegularExpression(@"^\d{10}$")]
        public string PhoneNumber { get; set; }
        public string Test { get; set; }
        
        [Required(ErrorMessage = "Sendika boş bırakılamaz.")]
        public int UnionBranchId { get; set; }
        
    }
}