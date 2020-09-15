using System.Collections.Generic;
using Sinav.Business.DTOs;
using Sinav.Data.Models;
using Sinav.Data.Models.SPResults;

namespace Sinav.Business.Services.UnionBranchServices
{
    public interface IUnionBranchService
    {
        void AddNewUnionBranch(UnionBranch branch);
        void DeleteUnionBranchById(int id);
        void UpdateUnionBranch(UnionBranch branch);
        PagedList<GetUnionsFiltered> GetUnionBranchesPaged(int pageNumber, int pageSize, string searchTerm);
        List<GetUnionsByCityDTO> GetUnionBranchesByCity(int cityId);
        List<City> GetCities();
        UnionBranch GetUnionBranchById(int id);
    }
}