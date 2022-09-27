using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;

namespace QuizService.Repositories
{
    public class BasicRepository<T> : IBasicRepository<T> where T : class
    {
        protected IDbConnection Connection;
        protected readonly string TableName;

        protected BasicRepository(IDbConnection dbConnection, string tableName)
        {
            Connection = dbConnection;
            TableName = tableName;
        }

        public async Task<T> GetByIdAsync(int id)
        {
            var sql = $"SELECT * FROM {TableName} WHERE Id = @Id;";
            Connection.Open();
            return await Connection.QuerySingleOrDefaultAsync<T>(sql, new { Id = id });
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            var sql = $"SELECT * FROM {TableName};";
            return await Connection.QueryAsync<T>(sql);
        }
    }
}
