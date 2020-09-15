using System.Collections.Generic;
using System.Threading.Tasks;
using Sinav.Data.Models;

namespace Sinav.Business.Services.StaffServices
{
    public interface IStaffService
    {
        List<Staff> GetStaffsAsync();
        List<Staff> GetStaffByOrganization(int organizationId);
        PagedList<Staff> GetStaffFiltered(int pageNumber, int pageSize, string searchTerm);
        Staff GetStaffById(int id);
        void UpdateStaff(Staff staff);
        void DeleteStaffById(int id);
        void NewStaff(string staffName, int orgId);

    }
}