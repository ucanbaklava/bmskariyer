using System.Collections.Generic;
using Sinav.Data.Context;

namespace Sinav.Data.Models
{
    public class Staff
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public Organization Organization { get; set; }
        public int OrganizationId { get; set; }
        public bool IsDeleted { get; set; } = false;
        public ICollection<AppUser> AppUsers { get; set; }

    }
}