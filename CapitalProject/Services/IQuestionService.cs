using CapitalProject.DTOs;
using CapitalProject.Models;

namespace CapitalProject.Services
{
    public interface IQuestionService
    {
        Task<Question> CreateQuestionAsync(QuestionDto question);
        Task<Question> UpdateQuestionAsync(string id, QuestionDto question);
        Task<List<Question>> GetQuestionsAsync();
        Task<Question> GetQuestionByIdAsync(string id);
    }
}
