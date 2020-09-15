using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sinav.Business.DTOs;
using Sinav.Data.Context;
using Sinav.Data.Models;

namespace Sinav.Business.Services.AnnouncementServices
{
    public class AnnouncementService : IAnnouncementService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<AnnouncementService> _logger;

        public AnnouncementService(AppDbContext context, ILogger<AnnouncementService> logger)
        {
            _context = context;
            _logger = logger;
        }
        public  void CreateAnnouncement(string content, string title, int? organizationId)
        {
            try
            {
                var announcement = new Announcement()
                {
                    Content = content,
                    Date = DateTime.Now,
                    Title = title.ToUpper(),
                    Slug = title.ToSlug()
                };

                announcement.OrganizationId = organizationId;

                _context.Announcements.Add(announcement);

                _context.SaveChanges();
                _logger.LogInformation("DUYURU - " + announcement.Title + " başlıklı duyuru oluşturuldu");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "DUYURU - Duyuru eklenirken hata oluştu");
                throw;
            }

        }

        public Announcement GetAnnouncementBySlug(string slug)
        {
            var announcement = _context.Announcements.FirstOrDefault(x => x.Slug.Equals(slug));
            return announcement;
        }

        public PagedList<Announcement> GetAllAnnouncements(int pageNumber, int pageSize, string searchTerm)
        {
            try
            {
                var allAnnouncements = PagedList<Announcement>.ToPagedList(
                    _context.Announcements.AsQueryable()
                        .Include(x=> x.Organization).OrderByDescending(x => x.Date),
                    pageNumber, 
                    pageSize);
                return allAnnouncements;
            }
            catch (Exception e)
            {
                _logger.LogError(e,"DUYURU -  Sayfalanmış duyurular getirilirken hata meydana geldi.");
                throw;
            }
        }
        
        public PagedList<Announcement> GetUserAnnouncements(int pageNumber, int pageSize, string searchTerm, string usid)
        {
            var orgId = _context.Users.Find(usid).OrganizationId;
            try
            {
                var allAnnouncements = PagedList<Announcement>.ToPagedList(
                    _context.Announcements.AsQueryable()
                        .Include(x=> x.Organization)
                        .Where(x => x.OrganizationId == orgId || x.OrganizationId == null).OrderByDescending(x => x.Date),
                    pageNumber, 
                    pageSize);
                return allAnnouncements;
            }
            catch (Exception e)
            {
                _logger.LogError(e,"DUYURU -  Sayfalanmış duyurular getirilirken hata meydana geldi.");
                throw;
            }
        }

        public void DeleteAnnouncementById(int id)
        {
            _context.Announcements.Find(id).IsDeleted = true;
            _context.SaveChanges();
            
        }

        public void UpdateAnnouncement(Announcement announcement)
        {
            var ann = _context.Announcements.Find(announcement.Id);
            ann.Content = announcement.Content;
            ann.Title = announcement.Title.ToUpper();
            ann.Slug = announcement.Title.ToSlug();

            _context.Announcements.Update(ann);
            _context.SaveChanges();
        }

        public Announcement GetAnnouncementById(int id)
        {
            return _context.Announcements.Find(id);
        }
    }
}