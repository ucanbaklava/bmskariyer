using System.Collections;
using System.Collections.Generic;
using Sinav.Data.Context;

namespace Sinav.Data.Models
{
    public class Organization
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string OrgImage { get; set; }
        public string Slug { get; set; }

        public ICollection<Staff> Staffs { get; set; }
        public ICollection<AppUser> AppUsers { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}