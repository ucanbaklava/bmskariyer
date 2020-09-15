using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sinav.Business.DTOs;
using Sinav.Data.Context;
using Sinav.Data.Models;

namespace Sinav.Business.Services.SubTopicServices
{
    public class SubTopicService: ISubTopicService
    {
        private readonly AppDbContext _context;

        public SubTopicService(AppDbContext context)
        {
            _context = context;
        }
        public PagedList<ListAllSubTopicsDto> GetAllSubTopics(int pageNumber, int pageSize, string searchTerm)
        {
            var allSubTopics = PagedList<ListAllSubTopicsDto>.ToPagedList(
                _context.SubTopic.Include(x => x.Subject).Select(x => new ListAllSubTopicsDto()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Subject = x.Subject.Name
                }).ToList().Where(x => new[] { x.Name, x.Subject}
                    .Any(s => s.Contains(searchTerm, StringComparison.InvariantCultureIgnoreCase))).AsQueryable(),
                pageNumber, 
                pageSize);
            return allSubTopics;
        }
        
        public async Task CreateSubTopic(string name, Stream file, string webrootpath, string path, int subjectId)
        {
            var newSubTopic = new SubTopic()
            {
                Name = name.ToUpper(),
                SubjectId = subjectId,
                Slug =  name.ToSlug()
            };
            var note = new LectureNote();
            var list = new List<LectureNote>();


            if (file != null)
            {
                var fileSize = SizeSuffix(file.Length);
                var fileStream = new FileStream(Path.Combine(webrootpath,path), FileMode.Create, FileAccess.Write);
                var extension = Path.GetExtension(fileStream.Name);

                file.Seek(0, SeekOrigin.Begin);
                await file.CopyToAsync(fileStream);
            
                fileStream.Dispose();
                note.Name = "denemeee";
                note.Path = "\\" + path;
                note.Size = fileSize;
                note.Excerpt = "";
                note.Extension = extension;
                note.Source = "";
                list.Add(note);

                newSubTopic.LectureNotes = list;

            }


            _context.SubTopic.Add(newSubTopic);

            await _context.SaveChangesAsync();
        }

        public async Task DeleteSubTopicById(int id)
        {
            _context.SubTopic.Find(id).IsDeleted = true;
            await _context.SaveChangesAsync();
        }

        public List<SubTopic> GetSubTopicsBySubject(int id)
        {
            return  _context.SubTopic.Where(x => x.SubjectId == id).ToList();
        }

        public SubTopic GetSubTopicById(int id)
        {
            return _context.SubTopic.Include(x => x.Subject).First(x => x.Id ==id);
        }

        public void UpdateSubTopic(SubTopic subTopic)
        {
            var subTopicToUpdate = _context.SubTopic.Find(subTopic.Id);
            subTopicToUpdate.Name = subTopic.Name.ToUpper();
            subTopicToUpdate.Slug = subTopic.Name.ToSlug();
            subTopicToUpdate.SubjectId = subTopic.SubjectId;

            _context.SaveChanges();
        }

        public List<LectureNote> GetLectureNotesBySubTopic(string slug)
        {
            var subject = _context.Subjects.FirstOrDefault(x => x.SubTopics.Any(y => y.Slug == slug));
            return _context.LectureNotes.Include(x=> x.SubTopic).Where(x => x.SubTopic.SubjectId == subject.Id).ToList();
        }

        public async Task AddDocToSubtopic(Stream file, string webrootpath, string path, int subtopicId, string name)
        {
            //            Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            var fileSize = SizeSuffix(file.Length);
            var asc = file.Length;
            var fileStream = new FileStream(Path.Combine(webrootpath,path), FileMode.Create, FileAccess.Write);
            var extension = Path.GetExtension(fileStream.Name);

            file.Seek(0, SeekOrigin.Begin);
            await file.CopyToAsync(fileStream);
            
            fileStream.Dispose();

            var subtopic = await _context.SubTopic.FindAsync(subtopicId);
            var lectureNote = new LectureNote()
            {
                Name = name,
                Path = "\\" + path,
                Extension = extension
            };
            subtopic.LectureNotes.Add(lectureNote);

            await _context.SaveChangesAsync();
        }

        public static string SizeSuffix(long value, int decimalPlaces = 0)
        {
            string[] SizeSuffixes = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
            if (value < 0)
            {
                throw new ArgumentException("Bytes should not be negative", "value");
            }
            var mag = (int)Math.Max(0, Math.Log(value, 1024));
            var adjustedSize = Math.Round(value / Math.Pow(1024, mag), decimalPlaces);
            return String.Format("{0} {1}", adjustedSize, SizeSuffixes[mag]);
        }
    }
}