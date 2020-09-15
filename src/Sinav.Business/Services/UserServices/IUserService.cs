using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Sinav.Business.DTOs;
using Sinav.Data.Context;
using Sinav.Data.Models.SPResults;

namespace Sinav.Business.Services.UserServices
{
    public interface IUserService
    {
        PagedList<GetUsersFiltered> GetAllUsers(int pageNumber, int pageSize, string searchTerm);
        Task ChangeUserStatus(string userId);
        UserService.UserSettingsViewModel UserSettings(string userId);
        void UpdateUserSettings(Stream stream, string firstName, string lastName, string userId, string webrootpath, string path);
    }
}