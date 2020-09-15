using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sinav.Business.Services.UnionBranchServices;
using Sinav.Data.Models;

namespace Sinav.Web.Controllers
{
    public class UnionBranchController : Controller
    {
        private readonly IUnionBranchService _unionBranchService;

        public UnionBranchController(IUnionBranchService unionBranchService)
        {
            _unionBranchService = unionBranchService;
        }

        [Authorize (Roles = "admin")]
        [Route("teskilat")]
        // GET
        public IActionResult Index()
        {
            return View();
        }
        [Authorize (Roles = "admin")]

        public IActionResult GetAllBranches()
        {
            var requestFormData = Request.Form;
            var start = Convert.ToInt32(requestFormData["start"].ToString());
            var draw = Convert.ToInt32(requestFormData["draw"].ToString());
            var pageSize = Convert.ToInt32(requestFormData["length"].ToString());
            var searchValue = requestFormData["search[value]"];
            var orderDir = requestFormData["order[0][dir]"];
            var branches = _unionBranchService.GetUnionBranchesPaged((start/pageSize)+1, pageSize, searchValue);
            var metadata = new
            {
                branches.TotalCount,
                branches.PageSize,
                branches.CurrentPage,
                branches.TotalPages,
                branches.HasNext,
                branches.HasPrevious
            };

            dynamic response = new
            {
                aaData = branches,
                draw = draw,
                RecordsFiltered = branches.TotalCount,
                iTotalRecords = branches.TotalCount,
            };
    
            return Json(response);
        }
        public IActionResult GetCities()
        {
            return Json(_unionBranchService.GetCities());
        }

        public IActionResult GetBranchesById(int cityId)
        {
            return Json(_unionBranchService.GetUnionBranchesByCity(cityId));
        }

        public IActionResult GetBranchById(int id)
        {
            return Json(_unionBranchService.GetUnionBranchById(id));
        }

        [Authorize (Roles = "admin")]

        public IActionResult NewUnionBranch(UnionBranch branch)
        {
            if (ModelState.IsValid)
            {
                _unionBranchService.AddNewUnionBranch(branch);
                return Ok();
            }

            return BadRequest();
        }
        [Authorize (Roles = "admin")]

        public IActionResult DeleteBranchById(int branchId)
        {
            _unionBranchService.DeleteUnionBranchById(branchId);
            return Ok();
        }

        [HttpPost]
        public IActionResult GetUnionBranchById(int id)
        {
            return Json(_unionBranchService.GetUnionBranchById(id));
        }
        [HttpPost]

        public IActionResult UpdateBranch(UnionBranch unionBranch)
        {
            _unionBranchService.UpdateUnionBranch(unionBranch);
            return Ok();
        }
    }
}