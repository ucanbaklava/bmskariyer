using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sinav.Business.DTOs;
using Sinav.Business.Services.ImageServices;
using Sinav.Data.Context;
using Sinav.Data.Models.SPResults;

namespace Sinav.Business.Services.UserServices
{
    public class UserService: IUserService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<UserService> _logger;
        private readonly IImageService _imageService;

        public UserService(AppDbContext context, ILogger<UserService> logger, IImageService imageService)
        {
            _context = context;
            _logger = logger;
            _imageService = imageService;
        }
        public PagedList<GetUsersFiltered> GetAllUsers(int pageNumber, int pageSize, string searchTerm)
        {
            var p0 = new SqlParameter("@searchTerm",  searchTerm);
            var p1 = new SqlParameter("@PageNumber", pageNumber -1);
            var p2 = new SqlParameter("@PageSize", pageSize);
            
            var resultSet = _context.GetUsersFiltered.FromSqlRaw("exec SP_GetUsersFiltered @SearchTerm, @PageNumber, @PageSize",
               p0, p1, p2).ToList();

            var leaderboard = PagedList<Data.Models.SPResults.GetUsersFiltered>.ToPagedListWithCount(resultSet, pageNumber, pageSize, resultSet.FirstOrDefault() == null ? 0 : resultSet.First().Total);
            return leaderboard;

        }

        public async Task ChangeUserStatus(string userId)
        {
            var user = _context.Users.FirstOrDefault(x => x.Id == userId);
            user.IsApproved = !user.IsApproved;
            await _context.SaveChangesAsync();
        }


        public UserSettingsViewModel UserSettings(string userId)
        {
            var u = _context.Users
                .Include(x => x.Organization)
                .Include(x => x.Staff)
                .First(x => x.Id == userId);
            var user = new UserSettingsViewModel()
            {
                Avatar = u.Avatar,
                Email = u.Email,
                FirstName = u.FirstName,
                LastName = u.LastName,
                OrgName = u.Organization.Name,
                StaffName = u.Staff.Name,
                UserId = u.Id
            };
            return user;
        }

        public void UpdateUserSettings(Stream stream, string firstName, string lastName, string userId, string webrootpath, string path)
        {
            var userToUpdate = _context.Users.Find(userId);
            if (stream != null)
            {
                var image = _imageService.SaveImage(stream, webrootpath, path);
                userToUpdate.Avatar = image;

            }

            if (userToUpdate.FirstName != firstName)
            {
                userToUpdate.FirstName = firstName;
            }
            if (userToUpdate.LastName != lastName)
            {
                userToUpdate.FirstName = lastName;
            }

            _context.Users.Update(userToUpdate);
            _context.SaveChanges();
        }

        public class UserSettingsViewModel
        {
            public string UserId { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string OrgName { get; set; }
            public string StaffName { get; set; }
            public string Avatar { get; set; }
            public string Email { get; set; }
        }


    }
}