using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Sinav.Data.Models;

namespace Sinav.Business.Services.OrganizationServices
{
    public interface IOrganizationService
    {
        List<Organization> GetOrganizations();
        Task CreateOrganization(Stream stream, string orgName, string webrootpath, string path);
        PagedList<Organization> GetOrganizationsFiltered(int pageNumber, int pageSize, string searchTerm);
        void DeleteOrganizationById(int id);
        Organization GetOrganizationById(int id);
        void UpdateOrganization(Stream stream, string orgName, int id, string webrootpath, string path);

    }
}