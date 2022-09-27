using System.Collections.Generic;
using System.Threading.Tasks;
using QuizService.Model.Domain;

namespace QuizService.Repositories
{
    public interface IAnswersRepository : IBasicRepository<Answer>
    {
        Task<Dictionary<int, IList<Answer>>> GetAnswerDictionaryByQuizId(int quizId);
    }
}
