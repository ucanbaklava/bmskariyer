using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sinav.Data.Context;

namespace Sinav.Web
{
    public class CookieEvents : CookieAuthenticationEvents
    {
        private readonly ILogger<CookieEvents> _logger;
        private readonly UserManager<AppUser> _userManager;

        private readonly IHttpContextAccessor _accessor;

        public CookieEvents(
            ILogger<CookieEvents> logger, UserManager<AppUser> userManager , IHttpContextAccessor accessor)
        {
            _logger = logger;
            _userManager = userManager;
            _accessor = accessor;
        }

        public override async Task SignedIn(CookieSignedInContext context)
        {

            var email = context.Principal.Identity.Name;

            var user = await _userManager.Users
                .Include(x => x.Organization)
                .SingleAsync(x => x.Email == email);

            if (user != null)
            {
                context.HttpContext.Request.HttpContext.Session.SetString("kurum", user.Organization.Name);
                context.HttpContext.Request.HttpContext.Session.SetString("kurumLogo", user.Organization.OrgImage ?? "");
                context.HttpContext.Request.HttpContext.Session.SetString("avatar", user.Avatar ?? "/assets/images/anonymous.png");
                context.HttpContext.Request.HttpContext.Session.SetString("ad", user.FirstName + " " + " " + user.LastName);
            }

            else
            {
                context.HttpContext.Request.HttpContext.Session.SetString("kurum", "");
                context.HttpContext.Request.HttpContext.Session.SetString("kurumLogo", "");
                context.HttpContext.Request.HttpContext.Session.SetString("avatar",  "/assets/images/anonymous.png");
                context.HttpContext.Request.HttpContext.Session.SetString("ad", "");
            }



            // context.HttpContext.Response.HttpContext.Session.SetString("kurum", user.Organization.Name.ToString());
            //context.HttpContext.Response.HttpContext.Session.SetString("avatar", user.Avatar);


        }
    }
}