using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuizService.Repositories
{
    public interface IBasicRepository<T> where T : class
    {
        Task<T> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
    }
}
