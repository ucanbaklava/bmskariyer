using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sinav.Business.DTOs;
using Sinav.Data.Context;
using Sinav.Data.Models;

namespace Sinav.Business.Services.PdfTestService
{
    public class PdfTestService: IPdfTestService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<PdfTestService> _logger;

        public PdfTestService(AppDbContext context, ILogger<PdfTestService> logger)
        {
            _context = context;
            _logger = logger;
        }
        
        
        public async Task CreatePdfTest(Stream file, string name, int subjectId, int? subTopicId, string answers, int time, string webrootpath, string path, int? orgId, bool overall)
        {
            try
            {
                var test = new PDFTest()
                {
                    Answers = answers,
                    Name = name.ToUpper(),
                    Time = time,
                    QuestionCount = answers.Split(',').Count(),
                    Slug =  name.ToSlug(),
                };

                if (overall)
                {
                    test.Overall = true;
                    test.OrganizationId = orgId;

                }
                else
                {
                    test.SubjectId = subjectId;
                    test.SubTopicId = subTopicId != 0 ? subTopicId : null;
                }
                
                

                var fileStream = new FileStream(Path.Combine(webrootpath, path), FileMode.Create, FileAccess.Write);
                var extension = Path.GetExtension(fileStream.Name);

                file.Seek(0, SeekOrigin.Begin);
                await file.CopyToAsync(fileStream);
            
                await fileStream.DisposeAsync();
                test.PdfPath = path;

                _context.PdfTest.Add(test);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                _logger.LogError(e, "Pdf Test Oluşturma");

            }

        }

        public List<PDFTest> GetPdfsByUserId(string userId)
        {
            var orgId = _context.Users.Find(userId).OrganizationId;
            var pdfTests = _context.PdfTest.Include(x=> x.Org).Where(x => x.OrganizationId ==  orgId || x.OrganizationId == null).ToList();
            return pdfTests;
        }

        public PagedList<PDFTest> GetAllTestsFiltered(int pageNumber, int pageSize, string searchTerm)
        {
            var allTests = PagedList<PDFTest>.ToPagedList(
                _context.PdfTest.Include(x => x.SubTopic).Include(x => x.Org).AsQueryable(),
                pageNumber,
                pageSize);
            return allTests;
        }

        public void DeleteTestById(int id)
        {
            var testToDelete = _context.PdfTest.Find(id);
            testToDelete.IsDeleted = true;
            _context.SaveChanges();
        }

        public DenemeSonuc GetTestResult(string slug, string answers, string userId)
        {
            var rightAnswerCount = 0;
            var emptyAnswerCount = 0;
            var rightAnswers = _context.PdfTest.Include(x => x.SubTopic).First(x => x.Slug == slug);
            var userAnswers = answers.Split(',');
            var correctAnswers = rightAnswers.Answers.Split(',');
            var a = rightAnswers.Answers;
            var b = a.Count();

            for (int i = 0; i < correctAnswers.Count(); i++)
            {
                if (userAnswers[i].ToLower() == "x")
                {
                    emptyAnswerCount++;
                }
                else if (userAnswers[i].ToLower() == correctAnswers[i].ToLower())
                {
                    rightAnswerCount++;
                }
            }


            var pdfTestResult = new PdfTestResult()
            {
                AppuserId = userId,
                RightAnswerCount = rightAnswerCount,
                PDFTestId = rightAnswers.Id,
                UserAnswer = answers,
                EmptyAnswerCount = emptyAnswerCount,
                WrongAnswerCount = userAnswers.Length - rightAnswerCount + emptyAnswerCount,
                Score = Math.Round((double)rightAnswerCount / correctAnswers.Count() * (double)100, 2)

            };
            _context.PdfTestResult.Add(pdfTestResult);
            _context.SaveChanges();

            var d = new DenemeSonuc()
            {
                CorrectAnswerCount = rightAnswerCount,
                EmptyAnswerCount = emptyAnswerCount,
                Right = rightAnswers.Answers.Split(','),
                User = userAnswers,
                QuestionCount = correctAnswers.Length
            };

            return d;

        }


        public List<PDFTest> GetTestBySubTopic(string slug)
        {
            var subject = _context.Subjects.First(x => x.Slug == slug);
            return _context.PdfTest.Include(x => x.SubTopic)
                .Where(x => x.SubTopic.SubjectId == subject.Id && x.SubTopicId != null ).ToList();
        }

        public List<PDFTest> GetSubjectTests(string slug)
        {
            var subject = _context.Subjects.First(x => x.Slug == slug);
            //var subject = _context.SubTopic.Include(x => x.Subject).FirstOrDefault(x => x.Slug == slug);
            return _context.PdfTest.Include(x => x.SubTopic).Where(x => x.SubjectId == subject.Id && x.SubTopicId == null).ToList();
        }

        public PagedList<Data.Models.SPResults.LeaderboardResult> GetLeaderboardResult(string slug, int pageNumber, int pageSize, string searchTerm)
        {
            var p0 = new SqlParameter("@Slug",  slug);
            var p1 = new SqlParameter("@SearchTerm", searchTerm);

            var p2 = new SqlParameter("@PageNumber", pageNumber -1);
            var p3 = new SqlParameter("@PageSize", pageSize);

            //var p4 = new SqlParameter("@Total", System.Data.SqlDbType.Int) { Direction = System.Data.ParameterDirection.Output };

            var resultSet = _context.LeaderboardResults.FromSqlRaw($"exec SP_GetLeaderboardFiltered  @Slug, @SearchTerm, @PageNumber, @PageSize",
                p0, p1, p2, p3).AsEnumerable().ToList();
            var leaderboard = PagedList<Data.Models.SPResults.LeaderboardResult>
                .ToPagedListWithCount(resultSet, pageNumber, pageSize, resultSet.FirstOrDefault() == null ? 0 : resultSet.First().Total);
            return leaderboard;
        }

        public PDFTest GetTestById(int id)
        {
            return _context.PdfTest.Find(id);
        }
        
        
    }
}