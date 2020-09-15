using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Sinav.Data.Context;
using Sinav.Data.Models;

namespace Sinav.Business.Services.StaffServices
{
    public class StaffService: IStaffService
    {
        private readonly AppDbContext _context;

        public StaffService(AppDbContext context)
        {
            _context = context;
        }
        public List<Staff> GetStaffsAsync()
        {
            throw new System.NotImplementedException();
        }

        public List<Staff> GetStaffByOrganization(int organizationId)
        {
            var resultSet = _context.Staff.Where(x => x.OrganizationId.Equals(organizationId)).ToList();
            return resultSet;
        }

        public PagedList<Staff> GetStaffFiltered(int pageNumber, int pageSize, string searchTerm)
        {
            if (searchTerm == null)
            {
                var staff = PagedList<Staff>.ToPagedList(
                    _context.Staff.Include(x=> x.Organization).AsQueryable(),
                    pageNumber, 
                    pageSize);
                return staff;
            }
            else
            {
                
                var staff = PagedList<Staff>.ToPagedList(
                    _context.Staff.Include(x=> x.Organization).ToList().Where(x => new[] { x.Name, x.Organization.Name}
                        .Any(s => s.Contains(searchTerm, StringComparison.InvariantCultureIgnoreCase))).AsQueryable(),
                    pageNumber, 
                    pageSize);
                return staff;
            }
        }

        public Staff GetStaffById(int id)
        {
            return _context.Staff.Find(id);
        }

        public void UpdateStaff(Staff staff)
        {
            if (string.IsNullOrEmpty(staff.Name))
            {
                throw new NullReferenceException();
            }
            var staffToUpdate = _context.Staff.Find(staff.Id);
            staffToUpdate.Name = staff.Name.ToUpper();
            _context.Staff.Update(staffToUpdate);
            _context.SaveChanges();

        }

        public void DeleteStaffById(int id)
        {
            _context.Staff.Find(id).IsDeleted = true;
            _context.SaveChanges();
        }

        public void NewStaff(string staffName, int orgId)
        {
            var staff = new Staff();
            staff.Name = staffName.ToUpper();
            staff.OrganizationId = orgId;

            if (_context.Organizations.Include(x => x.Staffs).First(x => x.Id == orgId)
                .Staffs.Any(x => x.Name == staffName)) throw new Exception();
            _context.Staff.Add(staff);
            _context.SaveChanges();


        }
    }
}