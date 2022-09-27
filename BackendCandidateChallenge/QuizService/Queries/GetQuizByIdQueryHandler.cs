using System.Data;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using QuizService.Model.Domain;
using QuizService.Repositories;

namespace QuizService.Queries
{
    public class GetQuizByIdQueryHandler : IRequestHandler<GetQuizByIdQuery, Quiz>
    {
        private readonly IQuizRepository _quizRepo;

        public GetQuizByIdQueryHandler(IDbConnection dbConnection)
        {
            _quizRepo = new QuizRepository(dbConnection, nameof(Quiz));
        }

        public async Task<Quiz> Handle(GetQuizByIdQuery request, CancellationToken cancellationToken)
        {
            return await _quizRepo.GetByIdAsync(request.Id).ConfigureAwait(false);
        }
    }
}
