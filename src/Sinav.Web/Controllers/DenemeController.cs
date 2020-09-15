using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Sinav.Business.Services.PdfTestService;
using Sinav.Web.DTOs;

namespace Sinav.Web.Controllers
{
    [Authorize(Policy = "Approved")]

    public class DenemeController : Controller
    {
        private readonly IPdfTestService _pdfTestService;
        private IWebHostEnvironment _hostEnvironment;

        public DenemeController(IPdfTestService pdfTestService, IWebHostEnvironment hostEnvironment)
        {
            _pdfTestService = pdfTestService;
            _hostEnvironment = hostEnvironment;

        }

        // GET
        [HttpPost]
        [Authorize(Roles = "admin")]
        public IActionResult Index(NewPdfTest test)
        {

            return Ok();
        }
    }
}