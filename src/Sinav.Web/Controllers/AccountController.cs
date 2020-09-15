using System;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Sinav.Business;
using Sinav.Business.Services.ImageServices;
using Sinav.Business.Services.MailServices;
using Sinav.Business.Services.OrganizationServices;
using Sinav.Business.Services.UserServices;
using Sinav.Data.Context;
using Sinav.Web.DTOs;
using Sinav.Web.ViewModels;
using ILogger = Serilog.ILogger;

namespace Sinav.Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        
        private readonly IImageService _imageService;
        private readonly IMailService _mailService;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IOrganizationService _organizationService;
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;
        private readonly ILogger<AccountController> _logger;

        public AccountController(UserManager<AppUser> userManager, 
            RoleManager<IdentityRole> roleManager,
            IImageService imageService, 
            IMailService mailService, 
            IWebHostEnvironment hostEnvironment,
            SignInManager<AppUser> signInManager,
            IOrganizationService organizationService,
            IUserService userService,
            IConfiguration configuration,
            AppDbContext context,
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _imageService = imageService;
            _mailService = mailService;
            _hostEnvironment = hostEnvironment;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _organizationService = organizationService;
            _userService = userService;
            _configuration = configuration;
            _context = context;
            _logger = logger;
        }
     /*
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Token(string email, string password)
        {
            if (ModelState.IsValid)
            {
                var userToVerify = await _userManager.FindByNameAsync(email);                

                // check the credentials
                if (await _userManager.CheckPasswordAsync(userToVerify, password))
                {
                    var claims = new[]
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, email),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                    };
                    await _signInManager.PasswordSignInAsync(userToVerify, password, false, false);

                    var token = new JwtSecurityToken
                    (
                        issuer: _configuration["Issuer"], //appsettings.json içerisinde bulunan issuer değeri
                        audience: _configuration["Audience"],//appsettings.json içerisinde bulunan audince değeri
                        claims: claims,
                        expires: DateTime.UtcNow.AddDays(1), // 30 gün geçerli olacak
                        notBefore: DateTime.UtcNow,
                        signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["SigningKey"])),//appsettings.json içerisinde bulunan signingkey değeri
                            SecurityAlgorithms.HmacSha256)
                    );
                    return Ok(new {token = new JwtSecurityTokenHandler().WriteToken(token)});                }
            }
            return BadRequest();
        }
*/

        [AllowAnonymous]
        public IActionResult Setup()
        {
            //_context.Database.Migrate();

            _context.Database.EnsureCreated();
           // _context.Database.Migrate();

            AppDbInitializer.SeedUsers(_userManager, _context);
            _context.Database.ExecuteSqlRaw(
                @"CREATE or alter VIEW View_CommonSubjects AS 
                                          select Subjects.Name, Subjects.Slug, Count(Questions.Id) as QuestionCount from Subjects
                                          inner join SubTopic on SubTopic.SubjectId = Subjects.Id
                                          inner join Questions on Questions.SubTopicId = SubTopic.Id
                                          where Subjects.OrganizationId IS NULL AND Questions.Published = 1 AND Questions.IsDeleted = 0 AND SubTopic.IsDeleted = 0
                                          group by Subjects.Name, Subjects.Slug");

            _context.Database.ExecuteSqlRaw(@"CREATE or alter PROCEDURE SP_FieldSubjects @FieldId int
                AS
                SELECT Subjects.Name, Subjects.Slug, COUNT(Questions.Id) as QuestionCount FROM Subjects
                    inner join SubTopic on SubTopic.SubjectId = Subjects.Id
                    inner join Questions on Questions.SubTopicId = SubTopic.Id
                    where Subjects.OrganizationId = @FieldId AND Questions.Published = 1 AND Questions.IsDeleted = 0 AND SubTopic.IsDeleted = 0 AND Subjects.IsDeleted = 0
                    group by Subjects.Name, Subjects.Slug");

            _context.Database.ExecuteSqlRaw(@"CREATE OR ALTER PROCEDURE SP_SolvedQuestionCountToday @UserId nvarchar(100)
                        AS
	                    select COUNT(Distinct Questions.Id)from Questions
	                    inner join UserAnswers on UserAnswers.QuestionId = Questions.Id
	                    inner join AspNetUsers on AspNetUsers.Id = UserAnswers.AppUserId
		                    where UserAnswers.AnswerDate = Convert(date,GETDATE()) and AspNetUsers.Id = @UserId");
            
            _context.Database.ExecuteSqlRaw(
                @"
                    create or alter procedure SP_GetSubTopicsBySlug @Slug nvarchar(250), @UserId nvarchar(250)
                        AS
                        select SubTopic.Name, Document.DocumentPath, SubTopic.Slug, COUNT(Distinct Questions.Id) as QuestionCount, COUNT(Distinct UserAnswers.QuestionId) as SolvedQuestionCount from SubTopic
                        inner join Subjects on Subjects.Id = SubTopic.SubjectId
                        left join Document on Document.Id = Subjects.DocumentId
                        inner join Questions on Questions.SubTopicId = SubTopic.Id
                        full outer join UserAnswers on UserAnswers.QuestionId  = Questions.Id
                        where Subjects.Slug = @Slug and SubTopic.IsDeleted = 0 and Questions.IsDeleted = 0
                        group by SubTopic.Name, Document.DocumentPath, SubTopic.Slug
                        ");

            _context.Database.ExecuteSqlRaw(
                @"
                    CREATE OR ALTER PROCEDURE SP_GetLeaderboardFiltered @Slug nvarchar(200), @SearchTerm nvarchar(100), @PageNumber int, @PageSize int
                    AS
                    ;WITH Main_CTE AS(
	                    select AspNetUsers.FirstName + ' ' + AspNetUsers.LastName as UserName, PdfTest.Name as TestName, PdfTestResult.Score from PdfTestResult
	                    inner join AspNetUsers on AspNetUsers.Id = PdfTestResult.AppuserId
	                    inner join PdfTest on PdfTest.Id = PdfTestResult.PDFTestId
	                    where PdfTest.Slug = @Slug and FirstName + LastName LIKE '%' + @SearchTerm + '%'
	                    ),
                    Count_CTE AS(
	                    SELECT COUNT(*) AS Total FROM Main_CTE
	                    )
                    SELECT * FROM Main_CTE, Count_CTE
                    order by Main_CTE.Score desc
                    OFFSET @PageNumber * @PageSize ROWS
	                    FETCH NEXT @PageSize ROWS ONLY"
            );            
                _context.Database.ExecuteSqlRaw(
                @"
                CREATE OR ALTER PROCEDURE SP_GetQuestionsFiltered  @SearchTerm nvarchar(100), @PageNumber int, @PageSize int
                AS
                ;WITH Main_CTE AS(
	                select Questions.Anonym, AspNetUsers.Avatar, Questions.Text as QuestionContent, Questions.Id as QuestionId, AspNetUsers.FirstName + ' ' + AspNetUsers.LastName as UserName,
	                SubTopic.Name as SubTopic, AspNetUsers.Id as UserId, Subjects.Name as Subject from Questions
	                inner join AspNetUsers on AspNetUsers.Id = Questions.AppuserId
	                inner join SubTopic on SubTopic.Id = Questions.SubTopicId
	                inner join Subjects on Subjects.Id = SubTopic.SubjectId
	                where Questions.Published= 1  and Questions.IsDeleted = 0 and (UPPER(FirstName + LastName) LIKE '%' + UPPER(@SearchTerm) + '%' or UPPER(Questions.Text) LIKE  '%' + UPPER(@SearchTerm) + '%' or UPPER(Subjects.Name) LIKE  '%' + UPPER(@SearchTerm) + '%'  or UPPER(SubTopic.Name) LIKE  '%' + UPPER(@SearchTerm) + '%' or Questions.Id LIKE  '%' + @SearchTerm + '%')
	                ),
                Count_CTE AS(
	                SELECT COUNT(*) AS Total FROM Main_CTE
	                )
                SELECT * FROM Main_CTE, Count_CTE
                order by  Main_CTE.QuestionId desc
                OFFSET @PageNumber * @PageSize ROWS
                FETCH NEXT @PageSize ROWS ONLY"
                );               
                _context.Database.ExecuteSqlRaw(
                @"
                CREATE OR ALTER PROCEDURE SP_GetQuestionFiltered  @SearchTerm nvarchar(100), @PageNumber int, @PageSize int
                AS
                ;WITH Main_CTE AS(
	                select Questions.Anonym, AspNetUsers.Avatar, Questions.Text as QuestionContent, Questions.Id as QuestionId, AspNetUsers.FirstName + ' ' + AspNetUsers.LastName as UserName,
	                SubTopic.Name as SubTopic, AspNetUsers.Id as UserId, Subjects.Name as Subject from Questions
	                inner join AspNetUsers on AspNetUsers.Id = Questions.AppuserId
	                inner join SubTopic on SubTopic.Id = Questions.SubTopicId
	                inner join Subjects on Subjects.Id = SubTopic.SubjectId
	                where Questions.Published= 1  and Questions.IsDeleted = 0 and (UPPER(FirstName + LastName) LIKE '%' + UPPER(@SearchTerm) + '%' or UPPER(Questions.Text) LIKE  '%' + UPPER(@SearchTerm) + '%' or UPPER(Subjects.Name) LIKE  '%' + UPPER(@SearchTerm) + '%'  or UPPER(SubTopic.Name) LIKE  '%' + UPPER(@SearchTerm) + '%' or Questions.Id LIKE  '%' + @SearchTerm + '%')
	                ),
                Count_CTE AS(
	                SELECT COUNT(*) AS Total FROM Main_CTE
	                )
                SELECT * FROM Main_CTE, Count_CTE
                order by  Main_CTE.QuestionId desc
                OFFSET @PageNumber * @PageSize ROWS
                FETCH NEXT @PageSize ROWS ONLY"
                );                
                _context.Database.ExecuteSqlRaw(
                @"
                CREATE OR ALTER PROCEDURE SP_GetUnionsFiltered  @SearchTerm nvarchar(100), @PageNumber int, @PageSize int
                AS
                ;WITH Main_CTE AS(
	                select UnionBranches.Curator, UnionBranches.Email, UnionBranches.Id, UnionBranches.Name, UnionBranches.Phone, Cities.Name as City from UnionBranches
	                inner join Cities on Cities.Id = UnionBranches.CityId
	                where UnionBranches.IsDeleted = 0 and (UPPER(UnionBranches.Curator) LIKE '%' + UPPER(@SearchTerm) + '%'  or UPPER(Cities.Name) LIKE  '%' + UPPER(@SearchTerm) + '%' or UPPER(UnionBranches.Name) LIKE  '%' + UPPER(@SearchTerm) + '%' or UnionBranches.Id LIKE  '%' + @SearchTerm + '%')
	                ),
                Count_CTE AS(
                SELECT COUNT(*) AS Total FROM Main_CTE
	                )
                SELECT * FROM Main_CTE, Count_CTE
                order by Main_CTE.Name 
                OFFSET @PageNumber * @PageSize ROWS
	                FETCH NEXT @PageSize ROWS ONLY"
                );               
                _context.Database.ExecuteSqlRaw(
                @"
                CREATE OR ALTER PROCEDURE SP_GetUsersFiltered  @SearchTerm nvarchar(100), @PageNumber int, @PageSize int
                AS
                ;WITH Main_CTE AS(
	                select AspNetUsers.Avatar, AspNetUsers.Email, AspNetUsers.FirstName, AspNetUsers.LastName, AspNetUsers.Gender, AspNetUsers.Id,
	                AspNetUsers.IsApproved, AspNetUsers.PhoneNumber, Organizations.Name as Organization, Staff.Name as Staff, UnionBranches.Name as UnionBranch from AspNetUsers
	                inner join Organizations on AspNetUsers.OrganizationId = Organizations.Id
	                inner join UnionBranches on UnionBranches.Id = AspNetUsers.UnionBranchId
	                inner join Staff on Staff.Id = AspNetUsers.StaffId
	                where FirstName + LastName LIKE '%' + @SearchTerm + '%' or UnionBranches.Name LIKE  '%' + @SearchTerm + '%' or Organizations.Name LIKE  '%' + @SearchTerm + '%' or AspNetUsers.Id LIKE  '%' + @SearchTerm + '%'
	                ),
                Count_CTE AS(
	                SELECT COUNT(*) AS Total FROM Main_CTE
	                )
                SELECT * FROM Main_CTE, Count_CTE
                order by IsApproved asc
                OFFSET @PageNumber * @PageSize ROWS
	                FETCH NEXT @PageSize ROWS ONLY"

                );

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> LogOut()
        {
            try
            {
                await _signInManager.SignOutAsync();
                // return RedirectToAction("Index", "Home");
                return Redirect("/giris");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Çıkış yaparken hata");
                return Redirect("/anasayfa");

            }
            

        }
        
        [AllowAnonymous]
        [Route("giris")]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.Title = "BMS Kariyer - Giriş";
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("giris")]
        public async Task<IActionResult> Login(UserLoginDTO userLogin)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userManager.FindByEmailAsync(userLogin.Email);
                    if (user == null)
                    {
                        ModelState.AddModelError("MailAddressCouldNotBeFound", "Bu mail adresine ait bir hesap bulunamadı.");
                        return View();
                    }

                    if (!await _userManager.CheckPasswordAsync(user, userLogin.Password))
                    {
                        ModelState.AddModelError("WrongCredentials", "Şifre veya kullanıcı adı hatalı.");
                        return View();
                    }
                    var result = await _signInManager.PasswordSignInAsync(user, userLogin.Password, false, false);

                    if (result.IsNotAllowed)
                    {
                        ModelState.AddModelError("Onay", 
                            "Mail adresinizi onayladığınızdan emin olup tekrar giriş yapmayı deneyin. Eğer onay maili gelmediyse 'Spam' klasörünüzü kontrol edin veya <a href='/onay-maili-gonder'>buraya</a> tıklayarak yeni bir doğrulama postası talep edin.  ");

                        return View();
                    }
                    if (result.Succeeded)
                    {
                        _logger.LogInformation("Giriş Başarılı");
                        return RedirectToAction("Index", "Home");

                    }

                    if (result.IsLockedOut)
                    {
                        ModelState.AddModelError("LockedOut", "Bu hesaba çok sayıda başarısız giriş denemesi yapıldığı için, geçici olarak uzaklaştırma verilmiştir. Lütfen daha sonra tekrar deneyin.");

                        return View();
                    }



                    //await _signInManager.PasswordSignInAsync(userLogin.Email, userLogin.Password, false, false);
                    //_logger.LogInformation("Kullanıcı Giriş Yaptı : {Ad} {Soyad} - {Email}", user.FirstName, user.LastName, user.Email);
                    else return View(userLogin);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Giriş Sırasında Hata USER ID= {0}", User.FindFirstValue(ClaimTypes.NameIdentifier));
                    return View(userLogin);

                }
            }
            else
            {
                return View(userLogin);
            }


        }

        [AllowAnonymous]
        [Route("kayit")]
        
        public IActionResult Register()
        {
            ViewBag.Title = "BMS Kariyer - Kayıt";

            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("kayit")]
        public async Task<IActionResult> Register(UserRegisterDTO newUser)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(newUser);
                }
                var user = new AppUser()
                {
                    UserName = newUser.Email,
                    Email = newUser.Email,
                    FirstName = newUser.FirstName,
                    LastName = newUser.LastName,
                    OrganizationId = newUser.OrganizationId,
                    StaffId = newUser.StaffId,
                    PhoneNumber = newUser.PhoneNumber,
                    Gender = newUser.Gender == 0,
                    UnionBranchId = newUser.UnionBranchId
                    /*Avatar = _imageService.SaveImage(newUser.Avatar.OpenReadStream(), 
                        _hostEnvironment.WebRootPath, 
                        Path.Combine("assets", "images", "users", newUser.FirstName + "-" + newUser.LastName + Path.GetExtension(newUser.Avatar.FileName)))*/
                };
                if (newUser.Test != null)
                {
                    var bytes = Convert.FromBase64String(newUser.Test.Split(",")[1]);
                    user.Avatar = _imageService.Base64ToImage(bytes, _hostEnvironment.WebRootPath,
                        Path.Combine("assets", "images", "users",
                           Guid.NewGuid() + Path.GetExtension(newUser.Avatar.FileName)));
                }


                var result = await _userManager.CreateAsync(user, newUser.Password);
                if (result.Succeeded)
                {
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    string confirmationLink = Url.Action("ConfirmEmail", "Account", new { userid = user.Id, token = token }, protocol: HttpContext.Request.Scheme);
                    var mailTemplate = Path.Combine(_hostEnvironment.WebRootPath, "assets/mailtemplate/email-confirmation.html");
                    var html = string.Empty;
                    using (StreamReader reader = System.IO.File.OpenText(mailTemplate))
                    {
                        html = reader.ReadToEnd();
                    }
                    html = html.Replace("@NAME", user.FirstName + " " + user.LastName).Replace("@CONFIRMATION", confirmationLink);
              
                   _mailService.SendConfirmationEmail(html, _configuration.GetSection("EmailSettings:Sender").Value, user.Email, _configuration.GetSection("EmailSettings:Password").Value );
                   //await _signInManager.SignInAsync(user, false);
                   return Redirect("/giris");
                }

                else
                {
                    ModelState.AddModelError(string.Empty, "Kayıt sırasında sorun oluştu, daha sonra tekrar deneyin.");
                    return View();
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Kayıt sırasında hata");
                throw;
            }

        }

        [AllowAnonymous]
        [Route("onay-maili-gonder")]
        public IActionResult SendActivationMail()
        {
            ViewBag.Title = "BMS Kariyer - Onay Maili";

            return View();
        }

        [AllowAnonymous]
        [Route("onay-maili-gonderildi")]
        public IActionResult ActivationMailSent()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        
        public async Task<IActionResult> SendActivationMailTo([FromForm]string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    ModelState.AddModelError("UserNameCouldNotBeFound", "Bu mail adresine ait bir hesap bulunamadı.");

                    return RedirectToAction("SendActivationMail", "Account");
                }
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                string confirmationLink = Url.Action("ConfirmEmail", "Account", new { userid = user.Id, token = token }, protocol: HttpContext.Request.Scheme);
                var mailTemplate = Path.Combine(_hostEnvironment.WebRootPath, "assets/mailtemplate/email-confirmation.html");
                var html = string.Empty;
                using (StreamReader reader = System.IO.File.OpenText(mailTemplate))
                {
                    html = reader.ReadToEnd();
                }
                html = html.Replace("@NAME", user.FirstName + " " + user.LastName).Replace("@CONFIRMATION", confirmationLink);
              
                _mailService.SendConfirmationEmail(html, _configuration.GetSection("EmailSettings:Sender").Value, user.Email, _configuration.GetSection("EmailSettings:Password").Value );
                _logger.LogInformation("{0} {1} {2} - Aktivasyon maili tekrar gönderildi", user.FirstName, user.LastName, user.Email);
                return RedirectToAction("ActivationMailSent", "Account");
            }
            catch (Exception e)
            {
                _logger.LogError( e, "Aktivasyon maili tekrar gönderilirken hata");
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<IActionResult> ResendActivationMail()
        {
            try
            {
                var user = _userManager.Users.First(x => x.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));
                if (user == null)
                {
                    ModelState.AddModelError("UserNameCouldNotBeFound", "Bu mail adresine ait bir hesap bulunamadı.");

                    return RedirectToAction("SendActivationMail", "Account");
                }
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                string confirmationLink = Url.Action("ConfirmEmail", "Account", new { userid = user.Id, token = token }, protocol: HttpContext.Request.Scheme);
                var mailTemplate = Path.Combine(_hostEnvironment.WebRootPath, "assets/mailtemplate/email-confirmation.html");
                var html = string.Empty;
                using (StreamReader reader = System.IO.File.OpenText(mailTemplate))
                {
                    html = reader.ReadToEnd();
                }
                html = html.Replace("@NAME", user.FirstName + " " + user.LastName).Replace("@CONFIRMATION", confirmationLink);
              
                _mailService.SendConfirmationEmail(html, _configuration.GetSection("EmailSettings:Sender").Value, user.Email, _configuration.GetSection("EmailSettings:Password").Value );
                _logger.LogInformation("{0} {1} {2} - Aktivasyon maili tekrar gönderildi", user.FirstName, user.LastName, user.Email);
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError( e, "Aktivasyon maili tekrar gönderilirken hata");
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel changePasswordViewModel)
        {
            try
            {
                if (changePasswordViewModel.NewPassword != changePasswordViewModel.NewPasswordConfirm)
                {
                    return BadRequest("Şifre ve şifre tekrar uyuşmuyor.");
                }
                var user = await  _userManager.Users.FirstAsync(x => x.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));
                if (!await _userManager.CheckPasswordAsync(user, changePasswordViewModel.Password))
                {
                    return BadRequest(
                        "Geçerli şifrenizi kontrol edin. Yeni şifrenizi en az 6 karakterden oluşmalıdır.");
                }
                var newPassword = _userManager.PasswordHasher.HashPassword(user, changePasswordViewModel.NewPassword);
                user.PasswordHash = newPassword;
                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return Ok("Şifreniz başarıyla değiştirilmiştir.");
                }

                return BadRequest("Bilgilerinizi kontrol edip tekrar deneyin.");


            }
            catch (Exception e)
            {
                _logger.LogInformation(e, "Şifre değiştirirken hata");
                return BadRequest("Hata, Daha Sonra Tekrar Deneyin");
            }

        }

        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId,string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if(result.Succeeded)
            {
                ViewBag.Message = "Email confirmed successfully!";
                return RedirectToAction("Settings", "Account");
            }
            else
            {
                ViewBag.Message = "Error while confirming your email!";
                return RedirectToAction("Settings", "Account");
            }
        }
        
        [Route("ayarlar")]
        public async Task<IActionResult> Settings()
        {
            ViewBag.Title = "BMS Kariyer - Ayarlar";

            
            ViewBag.Organizations = _organizationService.GetOrganizations();
            var user = _userService.UserSettings(User.FindFirstValue(ClaimTypes.NameIdentifier));
        
            return View(user);
        }
        
        [Authorize(Roles = "admin, ADMIN")]
        [Route("tum-kullanicilar")]
        public IActionResult ListUsers()
        {
            ViewBag.Title = "BMS Kariyer - Tüm Kullanıcılar";

            return View();
        }
        
        [Authorize(Roles = "admin, ADMIN")]
        [HttpPost]
        public IActionResult GetAllUsers()
        {
            var requestFormData = Request.Form;
            var start = Convert.ToInt32(requestFormData["start"].ToString());
            var draw = Convert.ToInt32(requestFormData["draw"].ToString());
            var pageSize = Convert.ToInt32(requestFormData["length"].ToString());
            var searchValue = requestFormData["search[value]"];
            var orderDir = requestFormData["order[0][dir]"];
            var users =_userService.GetAllUsers((start/pageSize)+1, pageSize, searchValue);
            var metadata = new
            {
                users.TotalCount,
                users.PageSize,
                users.CurrentPage,
                users.TotalPages,
                users.HasNext,
                users.HasPrevious
            };

            dynamic response = new
            {
                aaData = users,
                draw = draw,
                RecordsFiltered = users.TotalCount,
                iTotalRecords = users.TotalCount,
            };
    
            return Json(response);
        }

        [Authorize(Roles = "admin, ADMIN")]
        [HttpPost]
        public async Task<IActionResult> ChangeUserStatus(string userId)
        {
            await _userService.ChangeUserStatus(userId);
            return Ok();
        }


        [AllowAnonymous]
        [Route("/sifre-sifirlama")]
        public IActionResult ForgotPassword()
        {
            ViewBag.Title = "BMS Kariyer - Şifre Sıfırlama";

            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> SendPasswordResetLink([FromForm]string email)
        {
            try
            {
                var user = await  _userManager.FindByEmailAsync(email);
                
                if (user == null)
                {
                    ModelState.AddModelError("MailAddressCouldNotBeFound", "Girilen mail adresine ait bir hesap bulunamadı.");
                    return RedirectToAction("ForgotPassword", ModelState);
                }
                var token = await _userManager.GeneratePasswordResetTokenAsync(await _userManager.FindByEmailAsync(email));
            
                var resetLink = Url.Action("ResetPassword", 
                    "Account", new { token = token }, 
                    protocol: HttpContext.Request.Scheme);
                
                var mailTemplate = Path.Combine(_hostEnvironment.WebRootPath, "assets/mailtemplate/password-reset.html");
                var html = string.Empty;
                using (StreamReader reader = System.IO.File.OpenText(mailTemplate))
                {
                    html = reader.ReadToEnd();
                }
                html = html.Replace("@NAME", user.FirstName + " " + user.LastName).Replace("@CONFIRMATION", resetLink);
          
                _mailService.SendPasswordResetEmail(html, _configuration.GetSection("EmailSettings:Sender").Value, user.Email, _configuration.GetSection("EmailSettings:Password").Value );

                _logger.LogInformation("{0} {1} {2} - Şifre Sıfırlama Maili", user.FirstName, user.LastName, user.Email);

                return RedirectToAction("PasswordResetLinkSent", "Account");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            
            
        }
        
        [AllowAnonymous]

        public IActionResult ResetPassword(string token)
        {
            ViewBag.Title = "BMS Kariyer - Şifre Sıfırlama";

            return View();
        }
        
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel resetPassword)
        {
            if (ModelState.IsValid)
            {
                
                var user = await _userManager.FindByEmailAsync(resetPassword.UserName);


                var result = await _userManager.ResetPasswordAsync
                    (user, resetPassword.Token, resetPassword.Password);
                if (result.Succeeded)
                {
                    ViewBag.Message = "Password reset successful!";
                    return Redirect("/giris");
                }
                else
                {
                    ViewBag.Message = "Error while resetting the password!";
                    return View(resetPassword);
                }
            }

            else return View(resetPassword);

        }

        [AllowAnonymous]

        public IActionResult PasswordResetLinkSent()
        {
            ViewBag.Title = "BMS Kariyer - Şifre Sıfırlama";

            return View();
        }

        [Route("/upload")]
        [Authorize(Roles = "admin")]
        public IActionResult Upload(IFormFile file)
        {
            var image = _imageService.SaveImage(file.OpenReadStream(),_hostEnvironment.WebRootPath,
                Path.Combine("assets", "uploads",
                    Guid.NewGuid() + Path.GetExtension(file.FileName)) );

            return Ok(image);
        }

        public IActionResult ChangeUserInfo(UpdateUserSettings userSettings)
        {

            if (ModelState.IsValid)
            {
                if (userSettings.Image == null)
                {
                    _userService.UpdateUserSettings(null, userSettings.FirstName, userSettings.LastName, User.FindFirstValue(ClaimTypes.NameIdentifier),
                        _hostEnvironment.WebRootPath, "");
                    return RedirectToAction("Settings", "Account");

                }

                _userService.UpdateUserSettings(userSettings.Image.OpenReadStream(), userSettings.FirstName, userSettings.LastName, User.FindFirstValue(ClaimTypes.NameIdentifier),
                    _hostEnvironment.WebRootPath, Path.Combine("assets", "images", "users",
                        Guid.NewGuid() + Path.GetExtension(userSettings.Image.FileName)));
                return RedirectToAction("Settings", "Account");
            }

            return RedirectToAction("Settings", "Account");

        }
        
        public class UpdateUserSettings
        {
            [Required]
            [StringLength(60, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 2)]
            [Display(Name = "Ad")]
            public string FirstName { get; set; }
            [Required]
            [StringLength(60, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 2)]
            [Display(Name = "Soyad")]
            public string LastName { get; set; }
            public IFormFile? Image { get; set; }
        }

    }
}