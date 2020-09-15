using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sinav.Business.Services.SubTopicServices;
using Sinav.Data.Models;
using Sinav.Web.DTOs;

namespace Sinav.Web.Controllers
{
    [Authorize(Policy = "Approved")]

    public class SubTopicController : Controller
    {
        private readonly IWebHostEnvironment _hostEnvironment;

        private readonly ISubTopicService _subTopicService;
        private readonly ILogger<SubTopicController> _logger;

        public SubTopicController(ISubTopicService subTopicService, IWebHostEnvironment hostEnvironment, ILogger<SubTopicController> logger)
        {
            _subTopicService = subTopicService;
            _hostEnvironment = hostEnvironment;
            _logger = logger;
        }
        [Route("/alt-konular")]
        public IActionResult Index()
        {
            ViewBag.Title = "BMS Kariyer - Tüm Alt Konular";

            return View();
        }

        public IActionResult GetAllSubTopics()
        {
            var requestFormData = Request.Form;
            var start = Convert.ToInt32(requestFormData["start"].ToString());
            var draw = Convert.ToInt32(requestFormData["draw"].ToString());
            var pageSize = Convert.ToInt32(requestFormData["length"].ToString());
            var searchValue = requestFormData["search[value]"];
            var orderDir = requestFormData["order[0][dir]"];
            var subTopics = _subTopicService.GetAllSubTopics((start/pageSize)+1, pageSize, searchValue);
            var metadata = new
            {
                subTopics.TotalCount,
                subTopics.PageSize,
                subTopics.CurrentPage,
                subTopics.TotalPages,
                subTopics.HasNext,
                subTopics.HasPrevious
            };

            dynamic response = new
            {
                aaData = subTopics,
                draw = draw,
                RecordsFiltered = subTopics.TotalCount,
                iTotalRecords = subTopics.TotalCount,
            };
    
            return Json(response);
        }

        public IActionResult GetSubTopicsBySubjectId(int id)
        {
            return Json(_subTopicService.GetSubTopicsBySubject(id));
        }

        [HttpPost]
        public IActionResult UpdateSubTopic(SubTopic subTopic)
        {
            _subTopicService.UpdateSubTopic(subTopic);
            return Ok();
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> CreateNewSubTopic(NewSubTopicDTO subTopic)
        {
            await _subTopicService.CreateSubTopic(subTopic.Name, subTopic.SubTopicFile?.OpenReadStream(),
                _hostEnvironment.WebRootPath,
                Path.Combine("assets", "docs",
                    Path.GetRandomFileName() + Path.GetExtension(subTopic.SubTopicFile?.FileName)), subTopic.SubjectId);

            return Ok();
        }

        public async Task<IActionResult> DeleteSubTopic(int subTopicId)
        {
            try
            {
                await _subTopicService.DeleteSubTopicById(subTopicId);
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Alt Konu Silinirken Hata Meydana Geldi.");
                return BadRequest();
            }

        }

        public IActionResult GetSubTopicById(int id)
        {
            return Json(_subTopicService.GetSubTopicById(id));
        }
        
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> AddDocumentToSubtopic(AddDocToSubtopicDTO docToSubject)
        {
            await _subTopicService.AddDocToSubtopic(docToSubject.SubjectFile.OpenReadStream() , _hostEnvironment.WebRootPath,
                Path.Combine("assets", "documents", Path.GetRandomFileName() + Path.GetExtension(docToSubject.SubjectFile.FileName)),  docToSubject.SubTopicId, docToSubject.Name);
            return Ok();
        }

    }
}