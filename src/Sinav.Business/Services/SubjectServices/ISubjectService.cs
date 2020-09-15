using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Sinav.Business.DTOs;
using Sinav.Data.Models;
using Sinav.Data.Models.SPResults;

namespace Sinav.Business.Services.SubjectServices
{
    public interface ISubjectService
    {
        PagedList<ListAllSubjectsDto> GetAllSubjects(int pageNumber, int pageSize, string searchTerm);
        Task CreateSubject(string name, Stream file, int? organizationId, string webrootpath, string path);
        List<FieldSubject> GetFieldSubjects(string userId);
        List<CommonSubject> GetCommonSubjects();
        Task AddDocToSubject(Stream file, string webrootpath, string path, int subjectId);
        Task DeleteSubjectById(int subjectId);
        Subject GetSubjectById(int id);

        List<Subject> GetSubjects();
        List<GetSubTopicsBySlug> GetSubjectBySlug(string slug, string userId);
        List<Subject> GetSubjectsWithSubTopics();
        void UpdateSubject(Subject subject);
        List<Subject> GetSubjectsByOrganizationId(int id);
        List<Subject> CommonSubjects();

    }
}