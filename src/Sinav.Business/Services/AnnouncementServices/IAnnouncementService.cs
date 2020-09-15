using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sinav.Data.Models;

namespace Sinav.Business.Services.AnnouncementServices
{
    public interface IAnnouncementService
    {
        void CreateAnnouncement(string content, string title, int? organizationId);
        Announcement GetAnnouncementBySlug(string slug);
        PagedList<Announcement> GetAllAnnouncements(int pageNumber, int pageSize, string searchTerm);
        PagedList<Announcement> GetUserAnnouncements(int pageNumber, int pageSize, string searchTerm, string usid);

        void DeleteAnnouncementById(int id);
        void UpdateAnnouncement(Announcement announcement);
        Announcement GetAnnouncementById(int id);


    }
}