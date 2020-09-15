using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Sinav.Business.DTOs;
using Sinav.Data.Models;

namespace Sinav.Business.Services.SubTopicServices
{
    public interface ISubTopicService
    {
        PagedList<ListAllSubTopicsDto> GetAllSubTopics(int pageNumber, int pageSize, string searchTerm);
        Task CreateSubTopic(string name, Stream file, string webrootpath, string path, int subjectId);
        Task DeleteSubTopicById(int subTopicId);
        List<SubTopic> GetSubTopicsBySubject(int id);
        Task AddDocToSubtopic(Stream file, string webrootpath, string path, int subtopicId, string name);

        SubTopic GetSubTopicById(int id);
        void UpdateSubTopic(SubTopic subTopic);
        List<LectureNote> GetLectureNotesBySubTopic(string slug);

    }
}