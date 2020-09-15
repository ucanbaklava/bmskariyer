using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using Sinav.Business.DTOs;
using Sinav.Data.Context;

using Sinav.Data.Models;
using Sinav.Data.Models.SPResults;

namespace Sinav.Business.Services.QuestionServices
{
    
    public class QuestionService :  IQuestionService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<QuestionService> _logger;
        public QuestionService(AppDbContext context, ILogger<QuestionService> logger)
        {
            _context = context;
            _logger = logger;
        }


        public async Task CreateQuestion(int subTopicId, string qContent, string o1, string o2, string o3, string o4, string o5, string answer,string explanation, string cikmissoru, string userId)
        {
            try
            {
                var question = new Question()
                {
                    Text = qContent,
                    SubTopicId = subTopicId,
                    AppuserId = userId,
                    Explanation = String.IsNullOrEmpty(explanation) ? null: explanation,
                    CikmisSoru = cikmissoru == "on"
                };
                var optionList = new List<Options>()
                {
                    new Options() {Text = o1, isCorrect = answer == "a1", Question = question},
                    new Options() {Text = o2, isCorrect = answer == "a2", Question = question},
                    new Options() {Text = o3, isCorrect = answer == "a3", Question = question},
                    new Options() {Text = o4, isCorrect = answer == "a4", Question = question},
                    new Options() {Text = o5, isCorrect = answer == "a5", Question = question},
                };
                _context.Options.AddRange(optionList);
                _context.Questions.Add(question);
            
                await _context.SaveChangesAsync();
                
                _logger.LogInformation("{0} tarafından {1} ID'li konu için {2} ID'li soru oluşturuldu", userId, subTopicId, question.Id);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }


        }
        
        public async Task SendQuestion(int subTopicId, string qContent, string o1, string o2, string o3, string o4, string o5, string answer, string userId, bool anonym, bool cikmisSoru, string explanation)
        {

            try
            {
                var question = new Question()
                {
                    Text = qContent,
                    SubTopicId = subTopicId,
                    Published = false,
                    AppuserId = userId,
                    Anonym = anonym,
                    Explanation = explanation,
                    CikmisSoru = cikmisSoru
                };
                var optionList = new List<Options>()
                {
                    new Options() {Text = o1, isCorrect = answer == "a1", Question = question},
                    new Options() {Text = o2, isCorrect = answer == "a2", Question = question},
                    new Options() {Text = o3, isCorrect = answer == "a3", Question = question},
                    new Options() {Text = o4, isCorrect = answer == "a4", Question = question},
                    new Options() {Text = o5, isCorrect = answer == "a5", Question = question},
                };
                _context.Options.AddRange(optionList);
                _context.Questions.Add(question);
            
                await _context.SaveChangesAsync();
                _logger.LogInformation("{0} ID'li kullaınıcı tarafından {1} konusu için soru gönderildi.", userId, subTopicId);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }


        }

        public PagedList<GetQuestionDTO> GetUnpublishedQuestions(int pageNumber, int pageSize, string searchTerm)
        {
            
            var allSubjects = PagedList<GetQuestionDTO>.ToPagedList(
                _context.Questions.Include(x => x.AppUser).Where(x => x.Published == false).Select(x => new GetQuestionDTO()
                {
                    Options = x.Options,
                    QuestionContent = x.Text,
                    QuestionId = x.Id,
                    SubTopic = x.SubTopic.Name,
                    SubTopicId = x.SubTopicId,
                    SenderName = x.AppUser != null ? x.AppUser.FirstName + " " + x.AppUser.LastName : "Anonim",
                    Avatar = x.AppUser.Avatar ?? "/assets/images/anonymous.png",
                    Anonym = x.Anonym,
                    UserId =  x.AppuserId
                    
                }).AsQueryable(),
                pageNumber, 
                pageSize);
            return allSubjects;
        }

        public string GetExplanationById(int questionId)
        {
           return _context.Questions.Find(questionId).Explanation;
        }
        
        public QuestionResult CheckUserAnswer(int optionId)
        {
            var question = _context.Questions.Include(x => x.Options).FirstOrDefault(x => x.Options.Any(x => x.Id == optionId));
            var selectedOption = question.Options.First(x => x.Id == optionId);
            var qResult = new QuestionResult()
            {
                IsCorrect = selectedOption.isCorrect,
                CorrectAnswer = question.Options.First(
                    x => x.isCorrect).Text,
                UserAnswer = selectedOption.Text
            };

            return qResult;

        }

        public GetQuestionDTO GetRandomQuestion()
        {
           // var list =  _context.Questions.Include(li => li.Options).First();


            var q = _context.Questions.Include(x => x.Options).Include(x => x.SubTopic)
                .Select(x => new GetQuestionDTO()
                {
                    SubTopic = x.SubTopic.Name,
                    QuestionContent = x.Text,
                    QuestionId = x.Id,
                    Options = x.Options.ToList()
                }).FirstOrDefault();
            return q;
        }

        public PagedList<GetQuestionsFiltered> GetQuestions(int pageNumber, int pageSize, string searchTerm)
        {
            var p0 = new SqlParameter("@SearchTerm", searchTerm);
            var p1 = new SqlParameter("@PageNumber", pageNumber -1);
            var p2 = new SqlParameter("@PageSize", pageSize);


            var resultSet = _context.GetQuestionsFiltered.FromSqlRaw("exec SP_GetQuestionsFiltered @SearchTerm, @PageNumber, @PageSize",
                p0, p1, p2).ToList();
            var questions = PagedList<GetQuestionsFiltered>.ToPagedListWithCount(resultSet.AsQueryable(), pageNumber, pageSize, resultSet.FirstOrDefault() == null ? 0 : resultSet.First().Total);
            return questions;

        }
        

        public void SubmitUserAnswer(int questionId, int answerId, string userId)
        {
            var answer = _context.Options.Find(answerId);
            var userAnswer = new UserAnswer()
            {
                AnswerDate = DateTime.Now,
                QuestionId = questionId,
                IsTrue = answer.isCorrect,
                QuestionOptionId = answerId,
                AppUserId = userId
            };

            _context.UserAnswers.Add(userAnswer);
            _context.SaveChanges();
        }

        public List<PreviousAnswerDto> GetPreviousAnswers(string userId, int questionId)
        {
            var answers = _context.UserAnswers
                .Include(x => x.Question)
                .Where(x => x.AppUserId == userId && x.QuestionId == questionId)
                .Select(x => new PreviousAnswerDto()
            {
                Answer = _context.Options.FirstOrDefault(y => y.Id == x.QuestionOptionId).Text,
                AnswerDate = x.AnswerDate.ToString("dddd, dd MMMM yyyy HH:mm:ss"),
                AnswerId = x.Id,
                IsTrue = x.IsTrue
            }).ToList();
            return answers;
        }

        public List<GetQuestionDTO> GetQuestionsByOrganization(int count, int orgId,bool previouslyAsked, bool randomOrder, bool solvedFalse, bool notSolved, string userId )
        {
            var questions = _context.Questions
                .Include(x => x.AppUser)
                .Include(x => x.UserAnswers)
                .Include(x => x.SubTopic)
                .ThenInclude(x => x.Subject)
                .Where(x =>  x.Published && x.SubTopic.Subject.OrganizationId == orgId ).AsQueryable();

            if (solvedFalse)
            {
                questions = questions.Where(x => x.UserAnswers.Any(y => y.AppUserId == userId && y.IsTrue == false ) &&  x.UserAnswers.Any(y => y.IsTrue == false));
            }

            if (notSolved)
            {
                questions = questions.Where(x => !x.UserAnswers.Any());
            }
            
            if (previouslyAsked)
            {
                questions = questions.Where(x => x.CikmisSoru);
            }



            return questions.Select(x => new GetQuestionDTO()
            {
                Options = x.Options,
                QuestionContent = x.Text,
                QuestionId = x.Id,
                SubTopic = x.SubTopic.Name,
                Explanation = x.Explanation,
                Sender = x.Anonym  ? "Anonim" : x.AppUser.FirstName + " "  + x.AppUser.LastName,
                CikmisSoru = x.CikmisSoru
            }).OrderBy(x =>  Guid.NewGuid()).Take(count).ToList();
        }

        public List<GetQuestionDTO> GetQuestionsBySubTopic(string subTopicSlug)
        {
            var questions = _context.Questions
                .Include(x => x.SubTopic)
                .ThenInclude(x => x.Subject)
                .Where(x => x.SubTopic.Slug.Equals(subTopicSlug) && x.Published)
                .Select(x => new GetQuestionDTO()
                {
                    Options = x.Options,
                    QuestionContent = x.Text,
                    QuestionId = x.Id,
                    SubTopic = x.SubTopic.Name
                }).ToList();

            return questions;
        }


        public void DeleteQuestionById(int id)
        {
            _context.Questions.Find(id).IsDeleted = true;
            _context.SaveChanges();
        }

        public void ApproveQuestion(int subTopicId, string qContent, string o1, string o2, string o3, string o4, string o5,
            string answer, bool anonym, int questionId, string explanation, bool cikmisSoru)
        {
            var oldOptions = _context.Options.Where(x => x.QuestionId == questionId).ToList();
            _context.Options.RemoveRange(oldOptions);
            var question = _context.Questions.First(x => x.Id == questionId);

            question.Anonym = anonym;
            question.Text = qContent;
            question.Options = new List<Options>()
            {
                new Options() {Text = o1, isCorrect = answer == "a1"},
                new Options() {Text = o2, isCorrect = answer == "a2"},
                new Options() {Text = o3, isCorrect = answer == "a3"},
                new Options() {Text = o4, isCorrect = answer == "a4"},
                new Options() {Text = o5, isCorrect = answer == "a5"}
            };
            question.Published = true;
            question.ApprovedAt = DateTime.Now;
            question.SubTopicId = subTopicId;
            question.Explanation = explanation;
            question.CikmisSoru = cikmisSoru;

            _context.Questions.Update(question);


            _context.SaveChanges();
        }

        public void ReportQuestion(string feedback, int questionId, string userId)
        {
            var report = new QuestionReport()
            {
                Feedback = feedback,
                QuestionId = questionId,
                AppUserId = userId
            };

            _context.QuestionReports.Add(report);
            _context.SaveChanges();
        }

        public PagedList<ListQuestionReports> GetQuestionReports(int pageNumber, int pageSize)
        {
            var allReports = PagedList<ListQuestionReports>.ToPagedList(
                _context.QuestionReports.Include(x => x.AppUser).Select(x => new ListQuestionReports()
                {
                    Feedback = x.Feedback,
                    Id = x.Id,
                    QuestionId = x.QuestionId,
                    UserName = x.AppUser.FirstName + " " + x.AppUser.LastName,
                    Avatar = x.AppUser.Avatar ?? "/assets/images/anonymous.png",
                    Subject = x.Question.SubTopic.Subject.Name,
                    SubTopic = x.Question.SubTopic.Name

                    
                }).AsQueryable(),
                pageNumber, 
                pageSize);
            return allReports;
            
            
            
        }


        public List<GetQuestionDTO> GetQuestionsFiltered(bool previouslyAsked, bool randomOrder, bool solvedFalse, bool notSolved, int count, string slug, string userId)
        {
            var questions = _context.Questions
                .Include(x => x.AppUser)
                .Include(x => x.UserAnswers)
                .Include(x => x.SubTopic)
                .ThenInclude(x => x.Subject)
                .Where(x => x.SubTopic.Slug.Equals(slug) && x.Published).AsQueryable();

            if (solvedFalse)
            {
                questions = questions.Where(x => x.UserAnswers.Any(y => y.AppUserId == userId && y.IsTrue == false));
            }

            if (notSolved)
            {
                questions = questions.Where(x => !x.UserAnswers.Any());
            }
            
            if (previouslyAsked)
            {
                questions = questions.Where(x => x.CikmisSoru);
            }

            return questions.Select(x => new GetQuestionDTO()
            {
                Options = x.Options,
                QuestionContent = x.Text,
                QuestionId = x.Id,
                SubTopic = x.SubTopic.Name,
                Explanation = x.Explanation,
                Sender = x.Anonym  ? "Anonim" : x.AppUser.FirstName + " "  + x.AppUser.LastName,
                CikmisSoru = x.CikmisSoru
            }).OrderBy(x =>  Guid.NewGuid()).Take(count).ToList();
        }
        
        public List<GetQuestionDTO> AlanDisiKonular(bool previouslyAsked, bool randomOrder, bool solvedFalse, bool notSolved, int count, string slug, string userId)
        {
            var questions = _context.Questions
                .Include(x => x.AppUser)
                .Include(x => x.UserAnswers)
                .Include(x => x.SubTopic)
                .ThenInclude(x => x.Subject)
                .Where(x => x.Published && x.SubTopic.Subject.OrganizationId != null).AsQueryable();

            if (solvedFalse)
            {
                questions = questions.Where(x => x.UserAnswers.Any(y => y.AppUserId == userId && y.IsTrue == false));
            }

            if (notSolved)
            {
                questions = questions.Where(x => !x.UserAnswers.Any());
            }
            
            if (previouslyAsked)
            {
                questions = questions.Where(x => x.CikmisSoru);
            }
            
            return questions.Select(x => new GetQuestionDTO()
            {
                Options = x.Options,
                QuestionContent = x.Text,
                QuestionId = x.Id,
                SubTopic = x.SubTopic.Name,
                Explanation = x.Explanation,
                Sender = x.Anonym  ? "Anonim" : x.AppUser.FirstName + " "  + x.AppUser.LastName,
                CikmisSoru = x.CikmisSoru
            }).OrderBy(x =>  Guid.NewGuid()).Take(count).ToList();
        }
        
        public List<GetQuestionDTO> AlanIciKonular(bool previouslyAsked, bool randomOrder, bool solvedFalse, bool notSolved, int count, string slug, string userId)
        {
            var questions = _context.Questions
                .Include(x => x.AppUser)
                .Include(x => x.UserAnswers)
                .Include(x => x.SubTopic)
                .ThenInclude(x => x.Subject)
                .Where(x => x.Published && x.SubTopic.Subject.OrganizationId == null).AsQueryable();

            if (solvedFalse)
            {
                questions = questions.Where(x => x.UserAnswers.Any(y => y.AppUserId == userId && y.IsTrue == false));
            }

            if (notSolved)
            {
                questions = questions.Where(x => !x.UserAnswers.Any());
            }
            
            if (previouslyAsked)
            {
                questions = questions.Where(x => x.CikmisSoru);
            }
            
            return questions.Select(x => new GetQuestionDTO()
            {
                Options = x.Options,
                QuestionContent = x.Text,
                QuestionId = x.Id,
                SubTopic = x.SubTopic.Name,
                Explanation = x.Explanation,
                Sender = x.Anonym  ? "Anonim" : x.AppUser.FirstName + " "  + x.AppUser.LastName,
                CikmisSoru = x.CikmisSoru
            }).OrderBy(x =>  Guid.NewGuid()).Take(count).ToList();
        }

        public GetQuestionDTO GetQuestionById(int id)
        {
            return _context.Questions.Where(x => x.Id == id).Select(x => new GetQuestionDTO()
            {
                Anonym = x.Anonym,
                Options = x.Options,
                QuestionContent = x.Text,
                QuestionId = x.Id,
                SubTopic = x.SubTopic.Name,
                SubTopicId = x.SubTopic.Id,
                Explanation = x.Explanation,
                CikmisSoru = x.CikmisSoru
            }).First();
        }

    }
}