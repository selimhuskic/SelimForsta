using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using QuizService.Model.Domain;

namespace QuizService.Repositories
{
    public class AnswersRepository : BasicRepository<Answer>, IAnswersRepository
    {
        public AnswersRepository(IDbConnection dbConnection ,string tableName) : base(dbConnection, tableName)
        {
        }

        public async Task<Dictionary<int, IList<Answer>>> GetAnswerDictionaryByQuizId(int quizId)
        {
            const string answersSql = "SELECT a.Id, a.Text, a.QuestionId FROM Answer a INNER JOIN Question q ON a.QuestionId = q.Id WHERE q.QuizId = @QuizId;";

            Connection.Open();

            var result = await Connection.QueryAsync<Answer>(answersSql, new { QuizId = quizId });

            return result.Aggregate(new Dictionary<int, IList<Answer>>(), (dict, answer) =>
            {
                if (!dict.ContainsKey(answer.QuestionId))
                    dict.Add(answer.QuestionId, new List<Answer>());
                dict[answer.QuestionId].Add(answer);
                return dict;
            });
        }
    }
}
