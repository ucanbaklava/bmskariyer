using Microsoft.AspNetCore.Http;

namespace Sinav.Web.ViewModels
{
    public class UserSettingsViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string OrgName { get; set; }
        public string StaffName { get; set; }
        public string Avatar { get; set; }
    }
}