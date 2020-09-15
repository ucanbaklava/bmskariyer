using System.Collections.Generic;
using System.Threading.Tasks;
using Sinav.Business.DTOs;
using Sinav.Data.Models;
using Sinav.Data.Models.SPResults;

namespace Sinav.Business.Services.QuestionServices
{
    public interface IQuestionService
    {
        Task CreateQuestion(int subTopicId, string qContent,string o1, string o2, string o3, string o4, string o5, string answer,string explanation, string cikmissoru, string userId);
        QuestionResult CheckUserAnswer(int optionId);

        GetQuestionDTO GetRandomQuestion();
        PagedList<GetQuestionsFiltered> GetQuestions(int pageNumber, int pageSize, string searchTerm);
        void SubmitUserAnswer(int questionId, int answerId, string userId);
        List<PreviousAnswerDto> GetPreviousAnswers(string userId, int questionId);

        List<GetQuestionDTO> GetQuestionsByOrganization(int count, int orgId, bool previouslyAsked, bool randomOrder,
            bool solvedFalse, bool notSolved, string userId);
        List<GetQuestionDTO> GetQuestionsBySubTopic(string subTopicSlug);

        Task SendQuestion(int subTopicId, string qContent, string o1, string o2, string o3, string o4, string o5,
            string answer, string userId, bool anonym, bool cikmisSoru, string explanation);

        PagedList<GetQuestionDTO> GetUnpublishedQuestions(int pageNumber, int pageSize, string searchTerm);
        string GetExplanationById(int questionId);
        GetQuestionDTO GetQuestionById(int id);
        void DeleteQuestionById(int id);
        void ApproveQuestion(int subTopicId, string qContent, string o1, string o2, string o3, string o4, string o5, string answer, 
            bool anonym, int questionId, string explanation, bool cikmisSoru);

        void ReportQuestion(string feedback, int questionId, string userId);

        PagedList<ListQuestionReports> GetQuestionReports(int pageNumber, int pageSize);

        List<GetQuestionDTO> GetQuestionsFiltered(bool previouslyAsked, bool randomOrder, bool solvedFalse, bool notSolved, int count, string slug, string userId);

        List<GetQuestionDTO> AlanDisiKonular(bool previouslyAsked, bool randomOrder, bool solvedFalse, bool notSolved,
            int count, string slug, string userId);
        List<GetQuestionDTO> AlanIciKonular(bool previouslyAsked, bool randomOrder, bool solvedFalse, bool notSolved,
            int count, string slug, string userId);


    }
}