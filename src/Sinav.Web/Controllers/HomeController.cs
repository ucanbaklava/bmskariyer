using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog;
using Sinav.Business.Services.ImageServices;
using Sinav.Data.Context;

namespace Sinav.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IHostingEnvironment _hostEnvironment;
        private readonly AppDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IHostingEnvironment hostEnvironment, AppDbContext context, ILogger<HomeController> logger)
        {
            _hostEnvironment = hostEnvironment;
            _context = context;
            _logger = logger;
        }
        // GET
        [Route("anasayfa")]
        [Route("/")]

        public IActionResult Index()
        {
            ViewBag.Title = "BMS Kariyer - Anasayfa";

            return View();

        }



        [AllowAnonymous]
        [Route("iletisim")]
        public IActionResult Contact()
        {
            return View();
        }
        [AllowAnonymous]

        [Route("hakkimizda")]
        public IActionResult AboutUs()
        {
            return View();
        }
        [AllowAnonymous]

        [Route("uyelik-sozlesmesi")]

        public IActionResult Contract()
        {
            return View();
        }
        [AllowAnonymous]

        [Route("telif-hakki")]
        public IActionResult Copyright()
        {
            return View();
        }
        [AllowAnonymous]

        [Route("sorumluluk-reddi")]
        public IActionResult Disclaimer()
        {
            return View();
        }

    }
}