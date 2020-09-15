using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Server.HttpSys;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Serilog;
using Sinav.Business;
using Sinav.Business.Services.AnnouncementServices;
using Sinav.Business.Services.ImageServices;
using Sinav.Business.Services.MailServices;
using Sinav.Business.Services.OrganizationServices;
using Sinav.Business.Services.PdfTestService;
using Sinav.Business.Services.QuestionServices;
using Sinav.Business.Services.StaffServices;
using Sinav.Business.Services.SubjectServices;
using Sinav.Business.Services.SubTopicServices;
using Sinav.Business.Services.UnionBranchServices;
using Sinav.Business.Services.UserServices;
using Sinav.Data.Context;
using Sinav.Data.Models;
using Sinav.Web.Extensions;
using SixLabors.ImageSharp;

namespace Sinav.Web
{

    public class Startup
    {

        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
 
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {



            // Or you can also register as follows

            services.AddHttpContextAccessor();
            
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Seq("http://64.225.96.251:5341/", apiKey:"kLd0kLvvHo8mqEtMcBxO")
                .MinimumLevel.Information()
                .Enrich.WithProperty("AppName","BMS AKADEMİ")

                .CreateLogger();
            
            
            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromDays(1);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            
            services.AddDbContext<AppDbContext>(options =>
            {
                /*options.UseMySql(_configuration.GetConnectionString("MySQL"), mysqlOptions => 
                    mysqlOptions.ServerVersion(new Version(8,0,20), ServerType.MySql));*/
                options.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"));
            });



  
            

            services.AddIdentity<AppUser, IdentityRole>(options =>
                {
                    options.Password.RequireDigit = false;
                    options.Password.RequiredLength = 6;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequiredUniqueChars = 0;
                    options.Password.RequireNonAlphanumeric = false;

                    options.User.RequireUniqueEmail = true;
                    options.SignIn.RequireConfirmedEmail = true;
                    

                })
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders().AddClaimsPrincipalFactory<ApplicationClaimsIdentityFactory>();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("Approved", policy =>
                    policy.RequireClaim("IsApproved", "true"));
            });
            services.AddScoped<CookieEvents>();

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = new PathString("/giris");
                options.LogoutPath = "/";
                options.EventsType = typeof(CookieEvents);
                
                options.ExpireTimeSpan = TimeSpan.FromHours(24);
            });

            
           // services.AddScoped<IUserClaimsPrincipalFactory<AppUser>, ApplicationClaimsIdentityFactory>();


         /*   services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateActor = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = _configuration["Issuer"],
                        ValidAudience = _configuration["Audience"],
                        IssuerSigningKey =
                            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["SigningKey"]))
                    };
                });*/


            services.AddTransient<IImageService, ImageService>();
            
            services.AddSingleton<IMailService, MailService>();
            services.AddTransient<IOrganizationService, OrganizationService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IStaffService, StaffService>();
            services.AddTransient<ISubjectService, SubjectService>();
            services.AddTransient<ISubTopicService, SubTopicService>();
            services.AddTransient<IQuestionService, QuestionService>();
            services.AddTransient<IAnnouncementService, AnnouncementService>();
            services.AddTransient<IUnionBranchService, UnionBranchService>();

            services.AddTransient<IPdfTestService, PdfTestService>();

            services.AddControllersWithViews().AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore).AddRazorRuntimeCompilation();;

        }
        


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, UserManager<AppUser> userManager)
        {
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
          app.UseStatusCodePagesWithRedirects("/{0}.html");

          app.UseHsts();
          app.UseHttpsRedirection();


          app.UseStaticFiles();

          app.UseRouting();
          app.UseAuthentication();

          app.UseAuthorization();
          


          app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

        }
    }
 
}