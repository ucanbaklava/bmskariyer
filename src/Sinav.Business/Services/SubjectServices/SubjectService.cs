using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sinav.Business.DTOs;
using Sinav.Data.Context;
using Sinav.Data.Models;
using Sinav.Data.Models.SPResults;

namespace Sinav.Business.Services.SubjectServices
{
    public class SubjectService: ISubjectService
    {
        private readonly AppDbContext _context;

        public SubjectService(AppDbContext context)
        {
            _context = context;
        }
        public PagedList<ListAllSubjectsDto> GetAllSubjects(int pageNumber, int pageSize, string searchTerm)
        {
            var allSubjects = PagedList<ListAllSubjectsDto>.ToPagedList(
                _context.Subjects.Include(x => x.Document).Include(x => x.SubTopics).Select(x => new ListAllSubjectsDto()
                {
                    Id = x.Id,
                    Name = x.Name,
                    DocumentPath = x.Document.DocumentPath,
                    FileSize = x.Document.FileSize,
                    SubTopicCount = x.SubTopics.Count()
                }).ToList().Where(x => new[] { x.Name}
                .Any(s => s.Contains(searchTerm, StringComparison.InvariantCultureIgnoreCase))).AsQueryable(),
                pageNumber, 
                pageSize);
            return allSubjects;
        }

        public async Task CreateSubject(string name, Stream file,int? organizationId, string webrootpath, string path)
        {
            var doc = new Document();
            if (file != null)
            {
                var fileSize = SizeSuffix(file.Length);
                var fileStream = new FileStream(Path.Combine(webrootpath,path), FileMode.Create, FileAccess.Write);
                var extension = Path.GetExtension(fileStream.Name);

                file.Seek(0, SeekOrigin.Begin);
                await file.CopyToAsync(fileStream);
            
                fileStream.Dispose();

                doc.Name = "denemeee";
                doc.DocumentPath = "\\" + path;
                doc.FileSize = fileSize;
            }
            else
            {
                doc.Name = "denemeee";

            }
            
            var newSubject = new Subject()
            {
                Name = name.ToUpper(),
                Document = doc,
                OrganizationId = organizationId,
                Slug = name.ToSlug()
            };
            _context.Subjects.Add(newSubject);

            await _context.SaveChangesAsync();
        }

        public List<CommonSubject> GetCommonSubjects()
        {
            var fieldSubjects = _context.CommonSubjects.ToList();

            return fieldSubjects;
        }
        public List<Subject> CommonSubjects()
        {
            var commonSubjects = _context.Subjects.Where(x => x.OrganizationId == null).ToList();

            return commonSubjects;
        }
        public async Task AddDocToSubject(Stream file, string webrootpath, string path, int subjectId)
        {
            //            Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            var fileSize = SizeSuffix(file.Length);
            var asc = file.Length;
            var fileStream = new FileStream(Path.Combine(webrootpath,path), FileMode.Create, FileAccess.Write);
            var extension = Path.GetExtension(fileStream.Name);

            file.Seek(0, SeekOrigin.Begin);
            await file.CopyToAsync(fileStream);
            
            fileStream.Dispose();
            var doc = new Document()
            {
                Name = "asdasd",
                DocumentPath = "\\" + path,
                FileSize = fileSize
            };
            var subject = await _context.Subjects.FindAsync(subjectId);
            subject.Document = doc;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteSubjectById(int subjectId)
        {
            _context.Subjects.Find(subjectId).IsDeleted = true;
            await _context.SaveChangesAsync();
        }

        public Subject GetSubjectById(int id)
        {
            return _context.Subjects.Find(id);
        }

        public List<Subject> GetSubjects()
        {
            return _context.Subjects.Include(x => x.SubTopics).ToList();
        }

        public List<GetSubTopicsBySlug> GetSubjectBySlug(string slug, string userId)
        {
            List<GetSubTopicsBySlug> subTopics =
                _context.GetSubTopicsBySlug.FromSqlRaw("exec SP_GetSubTopicsBySlug {0}, {1}", slug, userId).ToList();
            return subTopics;
            //return _context.Subjects.Include(x => x.Document).Include(x => x.SubTopics).First(x => x.Slug == slug);
        }

        public List<FieldSubject> GetFieldSubjects(string userId)
        {
            var orgId = _context.Users.First(x => x.Id == userId).OrganizationId;
            List<FieldSubject> fieldSubjects =
                _context.FieldSubjects.FromSqlRaw("exec SP_FieldSubjects @FieldId={0}", orgId).ToList();
            return fieldSubjects;
        }

        public List<Subject> GetSubjectsWithSubTopics()
        {
            try
            {

                return _context.Subjects.Include(x => x.SubTopics).ToList();

            }
            catch (Exception e)
            {
                Console.WriteLine("****************************************************************************");
                Console.WriteLine(e);
                Console.WriteLine("****************************************************************************");

                throw;
            }
        }

        public void UpdateSubject(Subject subject)
        {
            var subjectToUpdate = _context.Subjects.Find(subject.Id);
            subjectToUpdate.Name = subject.Name;
            subjectToUpdate.Slug = subject.Name.ToSlug();
            subjectToUpdate.OrganizationId = subject.OrganizationId;
            _context.Subjects.Update(subjectToUpdate);
            _context.SaveChanges();
        }

        public List<Subject> GetSubjectsByOrganizationId(int id)
        {
            return _context.Subjects.Where(x => x.OrganizationId == id).ToList();
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