using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Sinav.Business.DTOs;
using Sinav.Data.Context;
using Sinav.Data.Models;
using Sinav.Data.Models.SPResults;

namespace Sinav.Business.Services.UnionBranchServices
{
    public class UnionBranchService : IUnionBranchService
    {
        private readonly AppDbContext _context;

        public UnionBranchService(AppDbContext context)
        {
            _context = context;
        }

        public void AddNewUnionBranch(UnionBranch branch)
        {
            _context.UnionBranches.Add(branch);
            _context.SaveChanges();
        }

        public void DeleteUnionBranchById(int id)
        {
            var branchToDelete = _context.UnionBranches.Find(id);
            _context.UnionBranches.Remove(branchToDelete);
            _context.SaveChanges();
        }

        public void UpdateUnionBranch(UnionBranch branch)
        {
            var branchToUpdate = _context.UnionBranches.Find(branch.Id);
            branchToUpdate.CityId = branch.CityId;
            branchToUpdate.Curator = branch.Curator;
            branchToUpdate.Email = branch.Email;
            branchToUpdate.Phone = branch.Phone;
            branchToUpdate.Name = branch.Name;
            _context.UnionBranches.Update(branchToUpdate);
            _context.SaveChanges();

        }

        public PagedList<GetUnionsFiltered> GetUnionBranchesPaged(int pageNumber, int pageSize, string searchTerm)
        {
            var p0 = new SqlParameter("@SearchTerm", searchTerm);
            var p1 = new SqlParameter("@PageNumber", pageNumber -1);
            var p2 = new SqlParameter("@PageSize", pageSize);

            var resultSet = _context.GetUnionsFiltered.FromSqlRaw($"exec SP_GetUnionsFiltered @SearchTerm, @PageNumber, @PageSize",
                p0, p1, p2).AsEnumerable().ToList();

            var unionBranches = PagedList<GetUnionsFiltered>.ToPagedListWithCount(resultSet, pageNumber, pageSize, resultSet.FirstOrDefault() == null ? 0 : resultSet.First().Total);
            return unionBranches;
        }

        public List<GetUnionsByCityDTO> GetUnionBranchesByCity(int cityId)
        {
            return _context.UnionBranches.Where(x => x.CityId == cityId).Select(x => new GetUnionsByCityDTO()
            {
                Id = x.Id,
                Name = x.Name
            }).ToList();
        }

        public List<City> GetCities()
        {
            return _context.Cities.OrderBy(x => x.Name).ToList();
        }

        public UnionBranch GetUnionBranchById(int id)
        {
            return _context.UnionBranches.Find(id);
        }
    }
}