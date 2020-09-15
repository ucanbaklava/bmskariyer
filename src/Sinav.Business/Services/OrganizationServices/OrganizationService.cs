using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sinav.Business.Services.ImageServices;
using Sinav.Data.Context;
using Sinav.Data.Models;

namespace Sinav.Business.Services.OrganizationServices
{
    public class OrganizationService: IOrganizationService
    {
        private readonly AppDbContext _context;
        private readonly IImageService _imageService;

        public OrganizationService(AppDbContext context, IImageService imageService)
        {
            _context = context;
            _imageService = imageService;
        }
        public  List<Organization> GetOrganizations()
        {

            var resultSet = _context.Organizations.ToList();
            return resultSet;
        }

        public async Task CreateOrganization(Stream stream, string orgName, string webrootpath, string path)
        {
            var image = _imageService.SaveImage(stream, webrootpath, path);
            var org = new Organization()
            {
                Name = orgName.ToUpper(),
                Slug = orgName.ToSlug(),
                OrgImage = image
            };
            _context.Organizations.Add(org);
            await _context.SaveChangesAsync();
        }

        public PagedList<Organization> GetOrganizationsFiltered(int pageNumber, int pageSize, string searchTerm)
        {
            if (searchTerm != null)
            {
                var allOrganizations = PagedList<Organization>.ToPagedList(
                    _context.Organizations.ToList().Where(x => new[] { x.Name}
                        .Any(s => s.Contains(searchTerm, StringComparison.InvariantCultureIgnoreCase))).AsQueryable(),
                    pageNumber, 
                    pageSize);
                return allOrganizations;
            }
            else
            {
                var allOrganizations = PagedList<Organization>.ToPagedList(
                    _context.Organizations.AsQueryable(),
                    pageNumber, 
                    pageSize);
                return allOrganizations;
            }

        }

        public void DeleteOrganizationById(int id)
        {
            _context.Organizations.Find(id).IsDeleted = true;
            _context.SaveChanges();
        }

        public Organization GetOrganizationById(int id)
        {
            return _context.Organizations.Find(id);

        }

        public void UpdateOrganization(Stream stream, string orgName, int id, string webrootpath, string path)
        {
            var orgToUpdate = _context.Organizations.Find(id);
            if (stream != null)
            {
                orgToUpdate.OrgImage = _imageService.SaveImage(stream, webrootpath, path);
            }

            if (orgToUpdate.Name != orgName)
            {
                orgToUpdate.Name = orgName.ToUpper();
            }

            _context.Update(orgToUpdate);
            _context.SaveChanges();
        }
    }
}