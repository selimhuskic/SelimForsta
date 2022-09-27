using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using QuizService.Model.Domain;
using QuizService.Repositories;

namespace QuizService.Queries
{
    public class GetQuestionsByIdQueryHandler : IRequestHandler<GetQuestionsByIdQuery, IEnumerable<Question>>
    {
        private readonly IQuestionRepository _questionRepo;

        public GetQuestionsByIdQueryHandler(IDbConnection dbConnection)
        {
            _questionRepo = new QuestionRepository(dbConnection, nameof(Question));
        }

        public async Task<IEnumerable<Question>> Handle(GetQuestionsByIdQuery request, CancellationToken cancellationToken)
        {
            return await _questionRepo.GetQuestionByQuizIdAsync(request.Id).ConfigureAwait(false);
        }
    }
}
