using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using QuizService.Model.Domain;
using QuizService.Repositories;

namespace QuizService.Queries
{
    public class GetAllQuizzesQueryHandler : IRequestHandler<GetAllQuizzesQuery, IEnumerable<Quiz>>
    {
        private readonly IQuizRepository _quizRepo;

        public GetAllQuizzesQueryHandler(IDbConnection dbConnection)
        {
            _quizRepo = new QuizRepository(dbConnection, nameof(Quiz));
        }

        public async Task<IEnumerable<Quiz>> Handle(GetAllQuizzesQuery request, CancellationToken cancellationToken)
        {
            return await _quizRepo.GetAllAsync().ConfigureAwait(false);
        }
    }
}
