using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sinav.Business.Services.AnnouncementServices;
using Sinav.Data.Models;
using Sinav.Web.DTOs;

namespace Sinav.Web.Controllers
{
    public class AnnouncementController : Controller
    {
        private readonly IAnnouncementService _announcementService;

        public AnnouncementController(IAnnouncementService announcementService)
        {
            _announcementService = announcementService;
        }
        // GET
        [Authorize(Roles = "admin")]
        [Route("duyurular")]
        public IActionResult Index()
        {
            ViewBag.Title = "BMS Kariyer - Tüm Duyurular";

            return View();
        }

        [Authorize]
        public IActionResult GetAnnouncements()
        {
            var requestFormData = Request.Form;
            var start = Convert.ToInt32(requestFormData["start"].ToString());
            var draw = Convert.ToInt32(requestFormData["draw"].ToString());
            var pageSize = Convert.ToInt32(requestFormData["length"].ToString());
            var searchValue = requestFormData["search[value]"];
            var orderDir = requestFormData["order[0][dir]"];
            var subjects = _announcementService.GetAllAnnouncements((start/pageSize)+1, pageSize, "");
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

        public IActionResult CreateAnnouncement(NewAnnouncementDTO announcement)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            _announcementService.CreateAnnouncement(announcement.Content, announcement.Title, announcement.OrganizationId);
           return Ok();
        }

        [Authorize]
        [Route("/duyuru/{slug}")]
        public IActionResult Announcement(string slug)
        {
            ViewBag.Title = "BMS Kariyer - Duyuru";

            var announcement = _announcementService.GetAnnouncementBySlug(slug);
            return View(announcement);
        }

        [Authorize(Roles = "admin")]
        public IActionResult DeleteAnnouncement(int announcementId)
        {
            _announcementService.DeleteAnnouncementById(announcementId);
            return Ok();
        }

        [Authorize(Roles = "admin")]
        public IActionResult UpdateAnnouncement(Announcement announcement)
        {
            try
            {
                _announcementService.UpdateAnnouncement(announcement);
                return Ok("Duyuru Güncellendi");
            }
            catch (Exception e)
            {
                
                return BadRequest("Duyuru Güncellenemedi");
            }
        }

        [Authorize(Roles = "admin")]
        public IActionResult GetAnnouncementById(int id)
        {
            return Json(_announcementService.GetAnnouncementById(id));
        }
    }
}