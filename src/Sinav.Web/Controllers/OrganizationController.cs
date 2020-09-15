using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Sinav.Business.Services.OrganizationServices;
using Sinav.Data.Models;
using Sinav.Web.DTOs;

namespace Sinav.Web.Controllers
{
    [Authorize]
    public class OrganizationController : Controller
    {
        private readonly IOrganizationService _organizationService;
        private readonly IWebHostEnvironment _hostEnvironment;
        public OrganizationController(IOrganizationService organizationService, IWebHostEnvironment hostEnvironment)
        {
            _organizationService = organizationService;
            _hostEnvironment = hostEnvironment;
        }
        // GET
        [AllowAnonymous]
        public IActionResult GetAllOrganizations()
        {
            var organizations = _organizationService.GetOrganizations();
            var resultSet = organizations.Select(x => new {x.Id, x.Name}).ToList();
            return Json(resultSet);
        }

        [Route("/kurumlar")]
        [Authorize(Roles = "admin")]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "admin")]
        public IActionResult GetAllOrganizationsFiltered()
        {
            var requestFormData = Request.Form;
            var start = Convert.ToInt32(requestFormData["start"].ToString());
            var draw = Convert.ToInt32(requestFormData["draw"].ToString());
            var pageSize = Convert.ToInt32(requestFormData["length"].ToString());
            var searchValue = requestFormData["search[value]"];
            var orderDir = requestFormData["order[0][dir]"];
            var orgs = _organizationService.GetOrganizationsFiltered((start/pageSize)+1, pageSize, searchValue);
            var metadata = new
            {
                orgs.TotalCount,
                orgs.PageSize,
                orgs.CurrentPage,
                orgs.TotalPages,
                orgs.HasNext,
                orgs.HasPrevious
            };

            dynamic response = new
            {
                aaData = orgs,
                draw = draw,
                RecordsFiltered = orgs.TotalCount,
                iTotalRecords = orgs.TotalCount,
            };
    
            return Json(response);
        }

        public IActionResult CreateOrganization()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> NewOrganization(NewOrganizationDTO organization)
        {
            await _organizationService.CreateOrganization(organization.Image.OpenReadStream(), organization.Name, _hostEnvironment.WebRootPath, Path.Combine("assets", "images", "kurumlar",
                Guid.NewGuid() + Path.GetExtension(organization.Image.FileName)));

            return Ok();
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public IActionResult DeleteOrgById(int id)
        {
            _organizationService.DeleteOrganizationById(id);
            return Ok();
        }

        public IActionResult GetOrgById(int id)
        {
            return Json(_organizationService.GetOrganizationById(id));
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public IActionResult UpdateOrganization(NewOrganizationDTO organization)
        {
            if (organization.Image != null)
            {
                _organizationService.UpdateOrganization(organization.Image.OpenReadStream(), organization.Name, organization.Id, _hostEnvironment.WebRootPath, Path.Combine("assets", "images", "kurumlar",
                    Guid.NewGuid() + Path.GetExtension(organization.Image.FileName )));
            }

            else
            {
                _organizationService.UpdateOrganization(null, organization.Name, organization.Id, _hostEnvironment.WebRootPath, Path.Combine("assets", "images", "kurumlar",
                    Guid.NewGuid() + Path.GetExtension(null)));
            }


            return Ok();
        }
    }
}