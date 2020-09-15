using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sinav.Business.DTOs;
using Sinav.Business.Services.QuestionServices;
using Sinav.Business.Services.SubjectServices;
using Sinav.Data.Context;
using Sinav.Data.Models;
using Sinav.Web.DTOs;
using ILogger = Serilog.ILogger;

namespace Sinav.Web.Controllers
{
    [Authorize(Policy = "Approved")]

    public class QuestionController : Controller
    {
        private readonly IQuestionService _questionService;
        private readonly UserManager<AppUser> _userManager;
        private readonly ISubjectService _subjectService;
        private readonly ILogger<QuestionController> _logger;
        public QuestionController(IQuestionService questionService, UserManager<AppUser> userManager, ISubjectService subjectService, ILogger<QuestionController> logger)
        {
            _questionService = questionService;
            _userManager = userManager;
            _subjectService = subjectService;
            _logger = logger;
        }
        // GET
        [Route("/soru/tum-sorular")]
        public IActionResult Index()
        {
            ViewBag.Subjects = _subjectService.GetSubjects();
            return View();
        }
        
        [HttpPost]
        [Authorize(Roles = "admin")]

        public IActionResult GetQuestions()
        {
            var requestFormData = Request.Form;
            var start = Convert.ToInt32(requestFormData["start"].ToString());
            var draw = Convert.ToInt32(requestFormData["draw"].ToString());
            var pageSize = Convert.ToInt32(requestFormData["length"].ToString());
            var searchValue = requestFormData["search[value]"];
            var orderDir = requestFormData["order[0][dir]"];
            var questions = _questionService.GetQuestions((start/pageSize)+1, pageSize, searchValue);
            var metadata = new
            {
                questions.TotalCount,
                questions.PageSize,
                questions.CurrentPage,
                questions.TotalPages,
                questions.HasNext,
                questions.HasPrevious
            };

            dynamic response = new
            {
                aaData = questions,
                draw = draw,
                RecordsFiltered = questions.TotalCount,
                iTotalRecords = questions.TotalCount,
            };
    
            return Json(response);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> AddQuestionToSubTopic(AddQuestionToSubTopic newQuestion)
        {
            var a = ModelState;
            if (ModelState.IsValid)
            {
                await _questionService.CreateQuestion(
                    newQuestion.SubTopicId,
                    newQuestion.Content,
                    newQuestion.a1,
                    newQuestion.a2,
                    newQuestion.a3,
                    newQuestion.a4,
                    newQuestion.a5,
                    newQuestion.Answer,
                    newQuestion.Explanation,
                    newQuestion.Cikmissoru,
                    User.FindFirstValue(ClaimTypes.NameIdentifier));

                return Ok();
            }

            return StatusCode(400, "Error");
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public IActionResult UpdateQuestion(AddQuestionToSubTopic newQuestion)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Konu Seçip Tüm Alanları Doldurunuz.");
            }
            _questionService.ApproveQuestion(newQuestion.SubTopicId, newQuestion.Content,
                newQuestion.a1, newQuestion.a2, newQuestion.a3, newQuestion.a4, 
                newQuestion.a5, newQuestion.Answer, newQuestion.Anonym == "on" , newQuestion.QuestionId, newQuestion.Explanation, newQuestion.Cikmissoru == "on");
            return Ok();
        }


        [Authorize]
        [HttpPost]
        public IActionResult GetQuestionsFiltered(QuestionFilterDTO filter)
        {
            return Json(_questionService.GetQuestionsFiltered(filter.PreviouslyAsked, filter.RandomOrder,
                filter.SolvedFalse, filter.NotSolved, filter.QuestionCount, filter.Slug, User.FindFirstValue(ClaimTypes.NameIdentifier)));
        }


        public IActionResult GetExplanation([FromQuery]int questionId)
        {
            return Json(_questionService.GetExplanationById(questionId));
        }

        public IActionResult ReportQuestion(ReportQuestion reportQuestion)
        {
            try
            {
                _questionService.ReportQuestion(reportQuestion.Feedback, reportQuestion.ReportId, User.FindFirstValue(ClaimTypes.NameIdentifier));
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Soru rapor edilirken hata");
                return BadRequest();
            }

        }

        [HttpPost]
        public IActionResult Submit(SubmitAnswerDTO answer)
        {
            try
            {
                _questionService.SubmitUserAnswer(answer.QuestionId, answer.AnswerId,  User.FindFirstValue(ClaimTypes.NameIdentifier));
                var result = _questionService.CheckUserAnswer(answer.AnswerId);

                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Soru cevabı submit edilirken hata meydana geldi.");
                return BadRequest("Cevabınız gönderilemedi. Daha sonra tekrar deneyin.");
            }
        }


        [Route("soru/alan-sorulari")]
        public IActionResult QuestionsByOrganization()
        {
            ViewBag.Title = "BMS Kariyer - Karışık Alan Soruları";

            return View();
        }
        [Route("soru/alan-disi-sorular")]
        public IActionResult CommonQuestions()
        {
            ViewBag.Title = "BMS Kariyer - Karışık Alan Dışı Sorular";

            return View();
        }

        public IActionResult AlanDisiKonular(QuestionFilterDTO filter)
        {
            return Json(_questionService.AlanDisiKonular(filter.PreviouslyAsked, filter.RandomOrder,
                filter.SolvedFalse, filter.NotSolved, filter.QuestionCount, filter.Slug, User.FindFirstValue(ClaimTypes.NameIdentifier)));
        }
        
        public IActionResult AlanIciKonular(QuestionFilterDTO filter)
        {
            return Json(_questionService.AlanIciKonular(filter.PreviouslyAsked, filter.RandomOrder,
                filter.SolvedFalse, filter.NotSolved, filter.QuestionCount, filter.Slug, User.FindFirstValue(ClaimTypes.NameIdentifier)));
        }
        public IActionResult GetQuestiondById(int id)
        {
            return Json(_questionService.GetQuestionById(id));
        }

        [Route("soru/{slug}")]
        public IActionResult QuestionsBySubTopic(string slug)
        {
            ViewBag.Slug = slug;
            //var questions = _questionService.GetQuestionsBySubTopic(slug);
            return View();
        }
        

        [HttpPost]
        public async Task<IActionResult> GetQuestionsByOrganization(QuestionFilterDTO filter)
        {
            if (filter.QuestionCount > 75)
            {
                return BadRequest();
            }
            var user = await _userManager.GetUserAsync(HttpContext.User);
            return Json(_questionService.GetQuestionsByOrganization(filter.QuestionCount, user.OrganizationId, filter.PreviouslyAsked, true, filter.SolvedFalse, filter.NotSolved, user.Id));
        }

        public IActionResult GetPreviousAnswers(int questionId)
        {
            var results = _questionService.GetPreviousAnswers(User.FindFirstValue(ClaimTypes.NameIdentifier), questionId);
            return Json(results);
        }

        public QuestionResult CheckUserAnswer(int optionId)
        {
            return _questionService.CheckUserAnswer(optionId);
        }

        [Route("soru-gonder")]
        public IActionResult SendQuestion()
        {
            ViewBag.Title = "BMS Kariyer - Soru Gönder";

            ViewBag.Subjects = _subjectService.GetSubjects();
            return View();
        }
        
        [HttpPost]
        public async  Task<IActionResult> SendQuestion(AddQuestionToSubTopic question)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            await _questionService.SendQuestion(
                question.SubTopicId,
                question.Content,
                question.a1,
                question.a2,
                question.a3,
                question.a4,
                question.a5,
                question.Answer,
                User.FindFirstValue(ClaimTypes.NameIdentifier),
                question.Anonym == "on",
                question.Cikmissoru == "on",
                question.Explanation);

            return Ok();
        }

        [Authorize(Roles = "admin")]
        [Route("/soru/gonderilen-sorular")]
        public IActionResult SentQuestions()
        {
            ViewBag.Title = "BMS Kariyer - Gönderilen Sorular";

            ViewBag.Subjects = _subjectService.GetSubjects();


            return View();
        }

        [HttpPost]
        [Authorize(Roles = "admin")]

        public IActionResult GetSentQuestions()
        {
            var requestFormData = Request.Form;
            var start = Convert.ToInt32(requestFormData["start"].ToString());
            var draw = Convert.ToInt32(requestFormData["draw"].ToString());
            var pageSize = Convert.ToInt32(requestFormData["length"].ToString());
            var searchValue = requestFormData["search[value]"];
            var orderDir = requestFormData["order[0][dir]"];
            var questions = _questionService.GetUnpublishedQuestions((start/pageSize)+1, pageSize, "");
            var metadata = new
            {
                questions.TotalCount,
                questions.PageSize,
                questions.CurrentPage,
                questions.TotalPages,
                questions.HasNext,
                questions.HasPrevious
            };

            dynamic response = new
            {
                aaData = questions,
                draw = draw,
                RecordsFiltered = questions.TotalCount,
                iTotalRecords = questions.TotalCount,
            };
    
            return Json(response);
        }

                
        [Authorize(Roles = "admin")]
        [Route("soru/raporlanan-sorular")]
        public IActionResult QuestionReports()
        {
            ViewBag.Title = "BMS Kariyer - Raporlanan Sorular";
            ViewBag.Subjects = _subjectService.GetSubjects();
            return View();
        }

        [Authorize(Roles = "admin")]
        public IActionResult GetQuestionReports()
        {
            var requestFormData = Request.Form;
            var start = Convert.ToInt32(requestFormData["start"].ToString());
            var draw = Convert.ToInt32(requestFormData["draw"].ToString());
            var pageSize = Convert.ToInt32(requestFormData["length"].ToString());
            var searchValue = requestFormData["search[value]"];
            var orderDir = requestFormData["order[0][dir]"];
            var reports = _questionService.GetQuestionReports((start/pageSize)+1, pageSize);
            var metadata = new
            {
                reports.TotalCount,
                reports.PageSize,
                reports.CurrentPage,
                reports.TotalPages,
                reports.HasNext,
                reports.HasPrevious
            };

            dynamic response = new
            {
                aaData = reports,
                draw = draw,
                RecordsFiltered = reports.TotalCount,
                iTotalRecords = reports.TotalCount,
            };
    
            return Json(response);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public IActionResult DeleteQuestionById(int questionId)
        {
            try
            {
                _questionService.DeleteQuestionById(questionId);
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Soru silinirken hata");
                return BadRequest();
            }

        }
        
    }
}