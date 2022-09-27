using System.Data;
using QuizService.Model.Domain;

namespace QuizService.Repositories
{
    public class QuizRepository : BasicRepository<Quiz>, IQuizRepository
    {
        public QuizRepository(IDbConnection dbConnection, string tableName) : base(dbConnection, tableName)
        {
        }
    }
}
