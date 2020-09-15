using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Sinav.Business.DTOs;
using Sinav.Data.Models;

namespace Sinav.Business.Services.PdfTestService
{
    public interface IPdfTestService
    {
        Task CreatePdfTest(Stream file, string name, int subjectId, int? subTopicId, string answers, int time,
            string webrootpath, string path, int? orgId, bool overall);
        List<PDFTest> GetTestBySubTopic(string slug);
        List<PDFTest> GetSubjectTests(string slug);
        List<PDFTest> GetPdfsByUserId(string userId);
        PagedList<PDFTest> GetAllTestsFiltered(int pageNumber, int pageSize, string searchTerm);
        void DeleteTestById(int id);

        DenemeSonuc GetTestResult(string slug, string answers, string userId);
        PagedList<Data.Models.SPResults.LeaderboardResult> GetLeaderboardResult(string slug, int pageNumber, int pageSize, string searchTerm);
        PDFTest GetTestById(int id);


    }
}