using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sinav.Business.DTOs;
using Sinav.Business.Services.PdfTestService;
using Sinav.Data.Context;
using Sinav.Data.Models;
using Sinav.Web.DTOs;

namespace Sinav.Web.Controllers
{
    [Authorize(Policy = "Approved")]

    public class PdfController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IPdfTestService _pdfTestService;
        private readonly IWebHostEnvironment _hostEnvironment;

        public PdfController(AppDbContext context, IPdfTestService pdfTestService, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _pdfTestService = pdfTestService;
            _hostEnvironment = hostEnvironment;
        }

        // GET
        [Route("/test/{slug}")]
        [Authorize(Roles = "admin")]

        public IActionResult Index(string slug)
        {
            ViewBag.Slug = slug;
            return View();
        }

        [Authorize(Roles = "admin")]
        [Route("tum-testler")]
        public IActionResult AllTests()
        {
            return View();
        }

        [HttpPost]
        public IActionResult GetAllTests()
        {
            var requestFormData = Request.Form;
            var start = Convert.ToInt32(requestFormData["start"].ToString());
            var draw = Convert.ToInt32(requestFormData["draw"].ToString());
            var pageSize = Convert.ToInt32(requestFormData["length"].ToString());
            var searchValue = requestFormData["search[value]"];
            var orderDir = requestFormData["order[0][dir]"];
            var staff = _pdfTestService.GetAllTestsFiltered((start/pageSize)+1, pageSize, searchValue);
            var metadata = new
            {
                staff.TotalCount,
                staff.PageSize,
                staff.CurrentPage,
                staff.TotalPages,
                staff.HasNext,
                staff.HasPrevious
            };

            dynamic response = new
            {
                aaData = staff,
                draw = draw,
                RecordsFiltered = staff.TotalCount,
                iTotalRecords = staff.TotalCount,
            };
    
            return Json(response);
        }

        public IActionResult GetQuiz(string slug)
        {
            var t = _context.PdfTest.First(x => x.Slug == slug);
            var quiz = new PdfQuiz()
            {
                Name = t.Name,
                Time = t.Time,
                PdfPath = t.PdfPath,
                QuestionCount = t.Answers.Split(',').Select(sValue => sValue.Trim()).Count()
            };
            return Json(quiz);
        }

        public IActionResult GetTestById(int id)
        {
            return Json(_pdfTestService.GetTestById(id));
        }

        public IActionResult DeleteTestById(int id)
        {
            _pdfTestService.DeleteTestById(id);
            return Ok();
        }

        
        [Authorize(Roles = "admin")]
        [Route("pdf-test-ekle")]
        public IActionResult CreatePdfTest()
        {
            return View();
        }
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> CreateTest(NewPdfTest test)
        {
            await _pdfTestService.CreatePdfTest(test.Pdf.OpenReadStream(), test.Name, test.SubjectId,test.SubTopicId, test.Answers,
                test.Time, _hostEnvironment.WebRootPath, Path.Combine("assets", "pdftests",
                    Path.GetRandomFileName() + Path.GetExtension(test.Pdf.FileName)), test.OrgId, test.Overall == "on");
            return Ok();
        }
        
        [Authorize(Roles = "admin")]
        [HttpPost]
        public IActionResult NewTest(NewPdfTest test)
        {
            _pdfTestService.CreatePdfTest(test.Pdf.OpenReadStream(), test.Name, test.SubjectId,test.SubTopicId, test.Answers,
                test.Time, _hostEnvironment.WebRootPath, Path.Combine("assets", "pdftests",
                    Path.GetRandomFileName() + Path.GetExtension(test.Pdf.FileName)), test.OrgId, test.Overall == "on");
            return Ok();
        }

        [HttpPost]

        public IActionResult SubmitTest(GetPdfResult result)
        {

            var d = _pdfTestService.GetTestResult(result.Slug, result.Answers,
                User.FindFirstValue(ClaimTypes.NameIdentifier));

            return Json(d);
        }

        [Route("siralama/{slug}")]
        public IActionResult Leaderboard(string slug)
        {
            ViewBag.Slug = slug;
            return View();
        }

        public IActionResult GetLeaderboardResult(string slug)
        {
            var requestFormData = Request.Form;
            var start = Convert.ToInt32(requestFormData["start"].ToString());
            var draw = Convert.ToInt32(requestFormData["draw"].ToString());
            var pageSize = Convert.ToInt32(requestFormData["length"].ToString());
            var searchValue = requestFormData["search[value]"];
            var orderDir = requestFormData["order[0][dir]"];
            var leaderboard = _pdfTestService.GetLeaderboardResult(slug,(start/pageSize)+1, pageSize, searchValue);
            var metadata = new
            {
                leaderboard.TotalCount,
                leaderboard.PageSize,
                leaderboard.CurrentPage,
                leaderboard.TotalPages,
                leaderboard.HasNext,
                leaderboard.HasPrevious
            };

            dynamic response = new
            {
                aaData = leaderboard,
                draw = draw,
                RecordsFiltered = leaderboard.TotalCount,
                iTotalRecords = leaderboard.TotalCount,
            };
    
            return Json(response);
        }
        
        public IActionResult Deneme(string slug)
        {
            var requestFormData = Request.Form;
            var start = Convert.ToInt32(requestFormData["start"].ToString());
            var draw = Convert.ToInt32(requestFormData["draw"].ToString());
            var pageSize = Convert.ToInt32(requestFormData["length"].ToString());
            var searchValue = requestFormData["search[value]"];
            var orderDir = requestFormData["order[0][dir]"];
            var leaderboard = _pdfTestService.GetLeaderboardResult(slug,(start/pageSize)+1, pageSize, searchValue);
            var metadata = new
            {
                leaderboard.TotalCount,
                leaderboard.PageSize,
                leaderboard.CurrentPage,
                leaderboard.TotalPages,
                leaderboard.HasNext,
                leaderboard.HasPrevious
            };

            dynamic response = new
            {
                aaData = leaderboard,
                draw = draw,
                RecordsFiltered = leaderboard.TotalCount,
                iTotalRecords = leaderboard.TotalCount,
            };
    
            return Json(response);
        }
    }
    


    public class PdfQuiz
    {
        public string Name { get; set; }
        public int Time { get; set; }
        public string SubtopicName { get; set; }
        public string PdfPath { get; set; }
        public int QuestionCount { get; set; }
    }

    public class GetPdfResult
    {
        public string Slug { get; set; }
        public string Answers { get; set; }
    }
}