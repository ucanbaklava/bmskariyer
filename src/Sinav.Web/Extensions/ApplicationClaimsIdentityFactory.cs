using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Sinav.Data.Context;

namespace Sinav.Web.Extensions
{
    public class ApplicationClaimsIdentityFactory: UserClaimsPrincipalFactory<AppUser, IdentityRole>
    {
        public ApplicationClaimsIdentityFactory(
            UserManager<AppUser> userManager
            , RoleManager<IdentityRole> roleManager
            , IOptions<IdentityOptions> optionsAccessor)
            : base(userManager, roleManager, optionsAccessor)
        { }

        public async override Task<ClaimsPrincipal> CreateAsync(AppUser user)
        {
            var principal = await base.CreateAsync(user);

            if (!string.IsNullOrWhiteSpace(user.FirstName))
            {
                ((ClaimsIdentity)principal.Identity).AddClaims(new[] {
                    new Claim(ClaimTypes.GivenName, user.FirstName)
                });
            }

            if (!string.IsNullOrWhiteSpace(user.LastName))
            {
                ((ClaimsIdentity)principal.Identity).AddClaims(new[] {
                    new Claim(ClaimTypes.Surname, user.LastName),
                });
            }


            if (user.IsApproved)
            {
                ((ClaimsIdentity)principal.Identity).AddClaims(new[] {
                    new Claim("IsApproved", "true")
                });
            }

            return principal;
        }
    }
}          

