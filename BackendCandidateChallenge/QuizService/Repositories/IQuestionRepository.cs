using System.Collections.Generic;
using System.Threading.Tasks;
using QuizService.Model.Domain;

namespace QuizService.Repositories
{
    public interface IQuestionRepository : IBasicRepository<Question>
    {
        Task<IEnumerable<Question>> GetQuestionByQuizIdAsync(int quizId);
    }
}
