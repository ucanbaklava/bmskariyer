using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Sinav.Data.Context;

namespace Sinav.Data.Models
{
    public class UnionBranch
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Şube adı boş geçilemez.")]
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Curator { get; set; }
        public City City { get; set; }

        [Required(ErrorMessage = "Şehir seçilmesi zorunludur.")]

        public bool IsDeleted { get; set; } = false;
        public int CityId { get; set; }

        public ICollection<AppUser> AppUsers { get; set; } = new List<AppUser>();
    }
}