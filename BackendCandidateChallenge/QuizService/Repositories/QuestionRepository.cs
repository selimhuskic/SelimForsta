using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using QuizService.Model.Domain;

namespace QuizService.Repositories
{
    public class QuestionRepository : BasicRepository<Question>, IQuestionRepository
    {
        public QuestionRepository(IDbConnection dbConnection, string tableName) : base(dbConnection, tableName)
        {
        }

        public async Task<IEnumerable<Question>> GetQuestionByQuizIdAsync(int quizId)
        {
            var sql = $"SELECT * FROM {TableName} WHERE QuizId = @QuizId;";
            Connection.Open();
            return await Connection.QueryAsync<Question>(sql, new { QuizId = quizId });
        }
    }
}
