using System;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sinav.Business.Services.SubjectServices;
using Sinav.Data.Models;
using Sinav.Web.DTOs;
using ILogger = Serilog.ILogger;

namespace Sinav.Web.Controllers
{
    [Authorize(Policy = "Approved")]

    public class SubjectController : Controller
    {
        private readonly ISubjectService _subjectService;
        private readonly IHostingEnvironment _hostEnvironment;
        private readonly ILogger<SubjectController> _logger;

        public SubjectController(ISubjectService subjectService, IHostingEnvironment hostEnvironment, ILogger<SubjectController> _logger)
        {
            _subjectService = subjectService;
            _hostEnvironment = hostEnvironment;
            _logger = _logger;
        }
        // GET
        [Authorize(Roles = "admin")]
        [Route("/konular")]
        public IActionResult Index()
        {
            ViewBag.Title = "BMS Kariyer - Tüm Konular";

            ViewBag.Subjects = _subjectService.GetSubjects();
            return View();
        }

        [Route("detay/{slug}")]
        public IActionResult SubjectDetail(string slug)
        {
            ViewBag.Title = "BMS Kariyer - Konu";
            
            ViewBag.Slug = slug;
            var subject = _subjectService.GetSubjectBySlug(slug, User.FindFirstValue(ClaimTypes.NameIdentifier));
            return View(subject);
        }
        


        [Authorize(Roles = "admin")]

        public async Task<IActionResult> DeleteSubject(int subjectId)
        {
            try
            {
                await _subjectService.DeleteSubjectById(subjectId);
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Konu silinirken hata meydana geldi.");
                return BadRequest();
            }

        }

        [Authorize(Roles = "admin")]

        public IActionResult GetAllSubjects()
        {
            var requestFormData = Request.Form;
            var start = Convert.ToInt32(requestFormData["start"].ToString());
            var draw = Convert.ToInt32(requestFormData["draw"].ToString());
            var pageSize = Convert.ToInt32(requestFormData["length"].ToString());
            var searchValue = requestFormData["search[value]"];
            var orderDir = requestFormData["order[0][dir]"];
            var subjects = _subjectService.GetAllSubjects((start/pageSize)+1, pageSize, searchValue);
            var metadata = new
            {
                subjects.TotalCount,
                subjects.PageSize,
                subjects.CurrentPage,
                subjects.TotalPages,
                subjects.HasNext,
                subjects.HasPrevious
            };

            dynamic response = new
            {
                aaData = subjects,
                draw = draw,
                RecordsFiltered = subjects.TotalCount,
                iTotalRecords = subjects.TotalCount,
            };
    
            return Json(response);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> CreateSubject(NewSubjectDTO newSubject)
        {
            var stream = newSubject.SubjectFile?.OpenReadStream();
            var fileName = newSubject.SubjectFile?.FileName;
            await _subjectService.CreateSubject(newSubject.Name , stream, newSubject.OrganizationId,
                _hostEnvironment.WebRootPath,
                Path.Combine("assets", "documents", Path.GetRandomFileName() + Path.GetExtension(fileName)));
            return Ok();
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> AddDocumentToSubject(AddDocToSubjectDTO docToSubject)
        {
            await _subjectService.AddDocToSubject(docToSubject.SubjectFile.OpenReadStream() , _hostEnvironment.WebRootPath,
                Path.Combine("assets", "documents", Path.GetRandomFileName() + Path.GetExtension(docToSubject.SubjectFile.FileName)),  docToSubject.SubjectId);
            return Ok();
        }

        public IActionResult GetSubjectsWSubTopics()
        {
            return Json(_subjectService.GetSubjectsWithSubTopics());
        }

        public IActionResult GetSubjects()
        {
            return Json(_subjectService.GetSubjects());
        }

        public IActionResult GetSubjectById(int id)
        {
            return Json(_subjectService.GetSubjectById(id));
        }

        [HttpPost]
        public IActionResult UpdateSubject(Subject subject)
        {
            
            _subjectService.UpdateSubject(subject);
            return Ok();
        }

        public IActionResult GetCommonSubjects()
        {
            return Json(_subjectService.CommonSubjects());
        }

        public IActionResult GetSubjectsbyorganizationid(int id)
        {
            return Json(_subjectService.GetSubjectsByOrganizationId(id));
        }
    }
}