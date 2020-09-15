using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sinav.Business.Services.StaffServices;
using Sinav.Data.Models;

namespace Sinav.Web.Controllers
{
    [Authorize]
    public class StaffController : Controller
    {
        private readonly IStaffService _staffService;

        public StaffController(IStaffService staffService)
        {
            _staffService = staffService;
        }

        [Authorize(Roles = "admin")]
        [Route("/kadrolar")]
        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult GetStaffsByOrganization([FromQuery]int organizationId)
        {
            var resultSet = _staffService.GetStaffByOrganization(organizationId);
            return Json(resultSet);
        }

        [HttpPost]
        public IActionResult GetStaffFiltered()
        {
            var requestFormData = Request.Form;
            var start = Convert.ToInt32(requestFormData["start"].ToString());
            var draw = Convert.ToInt32(requestFormData["draw"].ToString());
            var pageSize = Convert.ToInt32(requestFormData["length"].ToString());
            var searchValue = requestFormData["search[value]"];
            var orderDir = requestFormData["order[0][dir]"];
            var staff = _staffService.GetStaffFiltered((start/pageSize)+1, pageSize, searchValue);
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

        public IActionResult GetStaffById(int id)
        {
            return Json(_staffService.GetStaffById(id));
        }

        public IActionResult UpdateStaff(Staff staff)
        {
            _staffService.UpdateStaff(staff);
            return Ok();
        }

        public IActionResult DeleteStaffById(int id)
        {
            _staffService.DeleteStaffById(id);
            return Ok();
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public IActionResult NewStaff(Staff staff)
        {
            try
            {
                _staffService.NewStaff(staff.Name, staff.OrganizationId);
                return Ok("Kadro Kaydedildi");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest("Kadro kaydedilemedi");
            }

        }
    }
}