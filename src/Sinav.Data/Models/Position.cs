using System.Collections;
using System.Collections.Generic;
using Sinav.Data.Context;

namespace Sinav.Data.Models
{
    public class Position
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<AppUser> AppUsers { get; set; }
    }
}