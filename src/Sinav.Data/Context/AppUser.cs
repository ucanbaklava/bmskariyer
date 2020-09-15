using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Sinav.Data.Models;

namespace Sinav.Data.Context
{
    public class AppUser: IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public Organization Organization { get; set; }
        public int OrganizationId { get; set; }

        public Staff Staff { get; set; }

        public int StaffId { get; set; }
        public string Avatar { get; set; }
        public bool Gender { get; set; }
        public bool IsApproved { get; set; } = false;
        public UnionBranch UnionBranch { get; set; }
        public int UnionBranchId { get; set; }
        public string FullName()
        {
            return FirstName + " " + LastName;
        }

    }
}